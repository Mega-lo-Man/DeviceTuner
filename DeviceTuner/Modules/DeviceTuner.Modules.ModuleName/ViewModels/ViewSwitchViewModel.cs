using DeviceTuner.Core;
using DeviceTuner.Core.Mvvm;
using DeviceTuner.Modules.ModuleSwitch.Models;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

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

        private bool _sliderIsChecked = false;
        public bool SliderIsChecked
        {
            get { return _sliderIsChecked; }
            set { SetProperty(ref _sliderIsChecked, value); }
        }

        private ObservableCollection<EthernetSwitch> _switchList;
        public ObservableCollection<EthernetSwitch> SwitchList //Список коммутаторов
        {
            get { return _switchList; }
            set { SetProperty(ref _switchList, value); } 
        }

        #endregion

        private readonly IEventAggregator _ea;
        private readonly IDataRepositoryService _dataRepositoryService;
        private readonly INetworkTasks _networkTasks;
        private readonly Dispatcher _dispatcher;
        //private IMessageService _messageService;

        public ViewSwitchViewModel(IRegionManager regionManager,
                                    //IMessageService messageService,
                                    IDataRepositoryService dataRepositoryService,
                                    INetworkTasks networkTasks,
                                    IEventAggregator ea) : base(regionManager)
        {
            _ea = ea;
            _dataRepositoryService = dataRepositoryService;
            _networkTasks = networkTasks;
                        
            _ea.GetEvent<MessageSentEvent>().Subscribe(MessageReceived);

            SwitchList = new ObservableCollection<EthernetSwitch>();

            CheckedCommand = new DelegateCommand(async () => await StartCommandExecuteAsync(), StartCommandCanExecute);
            UncheckedCommand = new DelegateCommand(StopCommandExecute, StopCommandCanExecute);

            Title = "Switch"; // Заголовок вкладки

            _dispatcher = Dispatcher.CurrentDispatcher;

            //Message = messageService.GetMessage();
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
            if (SwitchList.Count > 0) // В списке для настройки есть коммутаторы?
                return true;
            else return false; 
        }

        private Task StartCommandExecuteAsync()
        {
            return Task.Run(() => DownloadLoop());
        }
        #endregion

        // Основной цикл - заливка в каждый коммутатор настроек из списка SwitchList
        private void DownloadLoop()
        {
            foreach (EthernetSwitch ethernetSwitch in SwitchList)
            {
                if (ethernetSwitch.Serial == null)//исключаем коммутаторы уже имеющие серийник (они уже были сконфигурированны)
                {
                    CurrentItemTextBox = ethernetSwitch.AddressIP;// Вывод адреса коммутатора в UI
                    
                    if (!_networkTasks.UploadConfigStateMachine(ethernetSwitch, GetSettingsDict()))
                        throw new Exception("Something went wrong in upload config procedure");
                    else _dataRepositoryService.SaveDevice(ethernetSwitch);
                    // Обновляем всю коллекцию d UI целиком
                    _dispatcher.BeginInvoke(new Action(() =>
                        {
                            CollectionViewSource.GetDefaultView(SwitchList).Refresh();
                        }));
                }
            }
            SliderIsChecked = false; // Всё! Залили настройки во все коммутаторы. Вырубаем слайдер (пололжение OFF)
        }
        
        // Формирование словаря с необходимыми данными для настройки коммутаторов (логин, пароль, адрес по умолчанию и т.п.)
        private Dictionary<string, string> GetSettingsDict()
        {
            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            settingsDict.Add("DefaultIPAddress", DefaultIP);
            settingsDict.Add("DefaultAdminLogin", DefaultLogin); 
            settingsDict.Add("DefaultAdminPassword", DefaultPassword);
            settingsDict.Add("NewAdminPassword", NewPassword);
            settingsDict.Add("NewAdminLogin", NewLogin);
            settingsDict.Add("IPmask", IPMask);
            return settingsDict;
        }
        
        private void MessageReceived(Tuple<int, string> message)
        {
            if (message.Item1 == MessageSentEvent.RepositoryUpdated)
            {
                SwitchList.Clear();
                foreach (EthernetSwitch item in _dataRepositoryService.GetDevices<EthernetSwitch>())
                {
                    SwitchList.Add(item);
                }
                /*foreach (EthernetSwitch item in _dataRepositoryService.GetSwitchDevices())
                {
                    SwitchList.Add(item);
                }*/
            }
            if(message.Item1 == MessageSentEvent.NeedOfUserAction)
            {
                MessageForUser = message.Item2;// Обновим информацию для пользователя 
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
