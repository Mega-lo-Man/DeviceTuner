using DeviceTuner.Core;
using DeviceTuner.Services;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace DeviceTuner.Modules.ModuleSwitch.Models
{
    public class NetworkTasks : INetworkTasks
    {
        private ushort _telnetPort = 23;
        private ushort _sshPort = 22;

        private IEventAggregator _ea;
        private ISender _telnetSender;
        private ISender _sshSender;

        public NetworkTasks(IEventAggregator ea, IEnumerable<ISender> senders)
        {
            _ea = ea;
            _telnetSender = senders.ElementAt(0);
            _sshSender = senders.ElementAt(1);
        }

        private void MessageForUser(string message)
        {
            //Сообщаем об обновлении данных в репозитории
            _ea.GetEvent<MessageSentEvent>().Publish(new Message {
                ActionCode = MessageSentEvent.NeedOfUserAction,
                MessageString = message
            });//(Tuple.Create(MessageSentEvent.NeedOfUserAction, message));
        }

        private void MessageToConsole(string message)
        {
            //Сообщаем об обновлении данных в репозитории
            _ea.GetEvent<MessageSentEvent>().Publish(new Message
            {
                ActionCode = MessageSentEvent.StringToConsole,
                MessageString = message
            });//(Tuple.Create(MessageSentEvent.NeedOfUserAction, message));
        }

        public bool SendPing(string NewIPAddress)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions
            {
                // Use the default Ttl value which is 128,
                // but change the fragmentation behavior.
                DontFragment = true
            };

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply;
            try
            {
                reply = pingSender.Send(NewIPAddress, timeout, buffer, options);
                if (reply.Status == IPStatus.Success) return true;
                else return false;
            }
            catch (Exception ex)
            {
                Debug.Print("Ping exception: " + ex.Message);
                return false;
            }
        }

        public bool UploadConfigStateMachine(EthernetSwitch switchDevice, Dictionary<string, string> settings)
        {
            MessageToConsole("Waiting device...");
            Dictionary<string, string> _sDict = settings;
            int State = 0;
            while (State < 6)
            {
                switch (State)
                {
                    case 0:
                        // Пингуем в цикле коммутатор по дефолтному адресу пока коммутатор не ответит на пинг
                        MessageForUser("Ожидание коммутатора");
                        if (SendPing(_sDict["DefaultIPAddress"])) State = 1;
                        break;
                    case 1:
                        // Пытаемся в цикле подключиться по Telnet (сервер Telnet загружается через некоторое время после успешного пинга)
                        if (_telnetSender.CreateConnection(_sDict["DefaultIPAddress"], 
                                                           _telnetPort, _sDict["DefaultAdminLogin"],
                                                           _sDict["DefaultAdminPassword"],
                                                           null))
                            State = 2;
                        break;
                    case 2:
                        // Заливаем первую часть конфига в коммутатор по Telnet
                        _telnetSender.Send(switchDevice, _sDict);
                        // Закрываем Telnet соединение
                        _telnetSender.CloseConnection();
                        State = 3;
                        break;
                    case 3:
                        // Пытаемся в цикле подключиться к SSH-серверу
                        if (_sshSender.CreateConnection(switchDevice.AddressIP,
                                                        _sshPort, _sDict["NewAdminLogin"],
                                                        _sDict["NewAdminPassword"],
                                                        @"id_rsa.key"))
                            State = 4;
                        break;
                    case 4:
                        // Заливаем вторую часть конфига по SSH-протоколу
                        _sshSender.Send(switchDevice, _sDict);
                        // Закрываем SSH-соединение
                        _sshSender.CloseConnection();
                        
                        MessageForUser("Замени коммутатор!");
                        State = 5;
                        break;
                    case 5:
                        // Пингуем в цикле коммутатор по новому IP-адресу (как только пинг пропал - коммутатор отключили)
                        if (!SendPing(switchDevice.AddressIP)) State = 6;
                        break;
                    case 6:
                        break;
                    default:
                        break;
                }
                // Go to пункт 1
            }
            return true;
        }
    }
}
