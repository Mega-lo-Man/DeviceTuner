﻿using DeviceTuner.Core;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using MinimalisticTelnet;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DeviceTuner.Services
{
    public class Telnet_Sender : ISender
    {
        private TelnetConnection _tc;
        private EventAggregator _ea;
        private Dictionary<string, string> _sDict;
        private EthernetSwitch _ethernetDevice;
        

        public Telnet_Sender(EventAggregator ea, TelnetConnection tc)
        {
            _ea = ea;
            _tc = tc;
        }

        public bool CloseConnection()
        {
            _tc.ConnectionClose();
            return true;
        }

        public bool CreateConnection(string IPaddress, ushort Port, string Username, string Password, string KeyFile)
        {
            if (_tc.CreateConnection(IPaddress, Port))
            {
                var ev = _ea.GetEvent<MessageSentEvent>();
                string returnStrFromConsole = _tc.Login(Username, Password, 1000);
                ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, returnStrFromConsole));
                // server output should end with "$" or ">" or "#", otherwise the connection failed
                string prompt = returnStrFromConsole.TrimEnd();
                prompt = returnStrFromConsole.Substring(prompt.Length - 1, 1);
                if (prompt != "$" && prompt != ">" && prompt != "#")
                    return false;
                return true;
            }
            else
                return false;
            
            
        }

        public EthernetSwitch Send(EthernetSwitch ethernetDevice, Dictionary<string, string> SettingsDict)
        {
            _sDict = SettingsDict;
            _ethernetDevice = ethernetDevice;
            
            PacketSendToTelnet(); // Передаём настройки по Telnet-протоколу
            _tc.ConnectionClose(); // Закрываем Telnet-соединение
            
            return ethernetDevice; // Возвращаем объект с заполненными свойствами полученными из коммутатора
        }

        private bool PacketSendToTelnet()
        {
            // Сократим начало выражения "_ea.GetEvent<MessageSentEvent>()" обозвав его "ev"
            MessageSentEvent ev = _ea.GetEvent<MessageSentEvent>();
            
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("conf t")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("hostname " + _ethernetDevice.Designation)));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("aaa authentication login default line")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("aaa authentication enable default line")));

            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("line console")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("login authentication default")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("enable authentication default")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("password " + _sDict["NewAdminPassword"])));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("exit")));

            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("ip ssh server")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("line ssh")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("login authentication default")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("enable authentication default")));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("password " + _sDict["NewAdminPassword"])));
            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("exit")));

            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("username " + _sDict["NewAdminLogin"] + " privilege 15 " + "password " + _sDict["NewAdminPassword"])));

            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("interface vlan 1")));

            ev.Publish(Tuple.Create(MessageSentEvent.StringToConsole, _tc.WriteRead("ip address " + _ethernetDevice.AddressIP + " /" + _sDict["IPmask"])));
            return true;
        }
    }
}
