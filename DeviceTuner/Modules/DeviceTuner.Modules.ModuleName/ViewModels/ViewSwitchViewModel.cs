using DeviceTuner.Core;
using DeviceTuner.Core.Mvvm;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTuner.Modules.ModuleName.ViewModels
{
    public class ViewSwitchViewModel : RegionViewModelBase
    {
        #region Properties
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _defaultLogin = "admin";
        public string DefaultLogin
        {
            get { return _defaultLogin; }
            set { SetProperty(ref _defaultLogin, value); }
        }

        private string _newLogin = "admin";
        public string NewLogin
        {
            get { return _newLogin; }
            set { SetProperty(ref _newLogin, value); }
        }

        private string _defaultPassword = "admin";
        public string DefaultPassword
        {
            get { return _defaultPassword; }
            set { SetProperty(ref _defaultPassword, value); }
        }

        private string _newPassword = "admin123";
        public string NewPassword
        {
            get { return _newPassword; }
            set { SetProperty(ref _newPassword, value); }
        }

        private string _defaultIP = "192.168.1.239";
        public string DefaultIP
        {
            get { return _defaultIP; }
            set { SetProperty(ref _defaultIP, value); }
        }

        private string _ipMask = "22";
        public string IPMask
        {
            get { return _ipMask; }
            set { SetProperty(ref _ipMask, value); }
        }

        private string _selectedDevice;
        public string SelectedDevice
        {
            get { return _selectedDevice; }
            set { SetProperty(ref _selectedDevice, value); }
        }

        private string _currentItemTextBox = "0";
        public string CurrentItemTextBox
        {
            get { return _currentItemTextBox; }
            set { SetProperty(ref _currentItemTextBox, value); }
        }

        private string _messageForUser = "1";
        public string MessageForUser
        {
            get { return _messageForUser; }
            set { SetProperty(ref _messageForUser, value); }
        }
        #endregion

        public ObservableCollection<NetworkDevice> SwitchList { get; set; } //Список коммутаторов

        private ISender _telnetSender;
        private ISender _sshSender;
        private IEventAggregator _ea;
        private IDataRepositoryService _dataRepositoryService;
        private ushort _telnetPort = 23;
        private ushort _sshPort = 22;
        //private IMessageService _messageService;

        public ViewSwitchViewModel(IRegionManager regionManager,
                                    IMessageService messageService,
                                    IDataRepositoryService dataRepositoryService,
                                    IEventAggregator ea,
                                    IEnumerable<ISender> senders) : base(regionManager)
        {
            _ea = ea;
            
            _dataRepositoryService = dataRepositoryService;
            _ea.GetEvent<MessageSentEvent>().Subscribe(MessageReceived);
            _telnetSender = senders.ElementAt(0);
            _sshSender = senders.ElementAt(1);

            SwitchList = new ObservableCollection<NetworkDevice>();

            CheckedCommand = new DelegateCommand(async () => await StartCommandExecuteAsync(), StartCommandCanExecute);
            UncheckedCommand = new DelegateCommand(StopCommandExecute, StopCommandCanExecute);

            Title = "Switch";
            Message = messageService.GetMessage();
        }

        #region Commands
        public DelegateCommand CheckedCommand { get; private set; }
        public DelegateCommand UncheckedCommand { get; private set; }

        private bool StopCommandCanExecute()
        {
            return true;//throw new NotImplementedException();
        }

        private void StopCommandExecute()
        {
            throw new NotImplementedException();
        }

        private bool StartCommandCanExecute()
        {
            if (SwitchList.Count > 0)
                return true;
            else return false;
        }

        private Task StartCommandExecuteAsync()
        {
            return Task.Run(() => DownloadLoop());
        }
        #endregion

        private void DownloadLoop()
        {
            foreach (NetworkDevice networkDevice in SwitchList)
            {
                CurrentItemTextBox = networkDevice.AddressIP;// Вывод адреса коммутатора
                if (!UploadConfigStateMachine(networkDevice)) throw new NotImplementedException("Something went wrong");
                else _dataRepositoryService.SaveDevice(networkDevice);
            }
        }

        private bool UploadConfigStateMachine(NetworkDevice nDevice)
        {
            //NetworkDevice _netDevice = nDevice;
            int State = 0;
            while (State < 6)
            {
                switch (State)
                {
                    case 0:
                        // Пингуем в цикле коммутатор по дефолтному адресу пока коммутатор не ответит на пинг
                        MessageForUser = "Ожидание коммутатора";
                        if (SendPing(DefaultIP)) State = 1;
                        break;
                    case 1:
                        // Пытаемся в цикле подключиться по Telenet (сервер Telnet загружается через некоторое время после успешного пинга)
                        if (_telnetSender.CreateConnection(DefaultIP, _telnetPort, DefaultLogin, DefaultPassword, null)) State = 2;
                        break;
                    case 2:
                        // Заливаем первую часть конфига в коммутатор по Telnet
                        _telnetSender.Send(nDevice, GetSettingsDict());
                        // Закрываем Telnet соединение
                        _telnetSender.CloseConnection();
                        State = 3;
                        break;
                    case 3:
                        // Пытаемся в цикле подключиться к SSH-серверу
                        if (_sshSender.CreateConnection(nDevice.AddressIP, _sshPort, NewLogin, NewPassword, @"id_rsa.key")) State = 4;
                        break;
                    case 4:
                        // Заливаем вторую часть конфига по SSH-протоколу
                        _sshSender.Send(nDevice, GetSettingsDict());
                        // Закрываем SSH-соединение
                        _sshSender.CloseConnection();
                        string RemoveDeviceStr = "Замени коммутатор!";
                        MessageForUser = RemoveDeviceStr;
                        State = 5;
                        break;
                    case 5:
                        // Пингуем в цикле коммутатор по новому IP-адресу (как только пинг пропал - коммутатор отключили)
                        if (!SendPing(nDevice.AddressIP)) State = 6;
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

        private bool SendPing(string NewIPAddress)
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

        private Dictionary<string, string> GetSettingsDict()
        {
            //throw new NotImplementedException();
            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            settingsDict.Add("DefaultIPAddress", DefaultIP);
            settingsDict.Add("DefaultAdminLogin", DefaultLogin); 
            settingsDict.Add("DefaultAdminPassword", DefaultPassword);
            settingsDict.Add("NewAdminPassword", NewPassword);
            settingsDict.Add("NewAdminLogin", NewLogin);
            settingsDict.Add("IPmask", IPMask);
            return settingsDict;
        }
        
        private void MessageReceived(Tuple<int, string> message/*string message*/)
        {
            if (message.Item1 == MessageSentEvent.RepositoryUpdated)
            {
                SwitchList.Clear();
                foreach (NetworkDevice item in _dataRepositoryService.GetSwitchDevices())
                {
                    if (item.Serial == null)//исключаем коммутаторы уже имеющие серийник (они уже были сконфигурированны)
                        SwitchList.Add(item);
                }
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
