using DeviceTuner.Core;
using DeviceTuner.Core.Mvvm;
using DeviceTuner.Modules.ModuleRS485.Models;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    public class ViewRS485ViewModel : RegionViewModelBase
    {
        private int DeviceCounter = 0;

        #region Commands
        public DelegateCommand StartCommand { get; private set; }
        #endregion

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _defaultIP;
        public string DefaultIP
        {
            get { return _defaultIP; }
            set { SetProperty(ref _defaultIP, value); }
        }

        private string _ipMask;
        public string IPMask
        {
            get { return _ipMask; }
            set { SetProperty(ref _ipMask, value); }
        }

        private string _defaultRS485Address = "127";
        public string DefaultRS485Address
        {
            get { return _defaultRS485Address; }
            set { SetProperty(ref _defaultRS485Address, value); }
        }

        private string _currentDeviceModel = "";
        public string CurrentDeviceModel
        {
            get{ return _currentDeviceModel; }
            set{ SetProperty(ref _currentDeviceModel, value); }
        }

        private string _serialTextBox;
        public string SerialTextBox 
        {
            get { return _serialTextBox; }
            set { SetProperty(ref _serialTextBox, value); }
        }

        private bool _isCheckedByCabinets = true;
        public bool IsCheckedByCabinets
        {
            get { return _isCheckedByCabinets; }
            set 
            {
                if (value)
                {
                    DevicesForProgramming.Clear(); // При переключении режима работы надо очистить список приборов для программирования
                }
                SetProperty(ref _isCheckedByCabinets, value);
            }
        }

        private bool _isCheckedByArea = false;
        public bool IsCheckedByArea
        {
            get { return _isCheckedByArea; }
            set 
            {
                if (value)
                {
                    DevicesForProgramming.Clear();// При переключении режима работы надо очистить список приборов для программирования
                    
                    foreach (RS485device item in _dataRepositoryService.GetAllDevices<RS485device>())
                    {
                        DevicesForProgramming.Add(item);
                    }
                    //CollectionViewSource.GetDefaultView(DevicesForProgramming).Refresh();
                }
                SetProperty(ref _isCheckedByArea, value);
            }
        }

        private bool _isCheckedComplexVerification = false;
        public bool IsCheckedComplexVerification
        {
            get { return _isCheckedComplexVerification; }
            set
            {
                if (value)
                {
                    DevicesForProgramming.Clear();// При переключении режима работы надо очистить список приборов для программирования
                }
                SetProperty(ref _isCheckedComplexVerification, value);
            }
        }

        private string _currentPort;
        public string CurrentPort
        {
            get { return _currentPort; }
            set { SetProperty(ref _currentPort, value); } 
        }

        private ObservableCollection<Cabinet> _cabinetList = new ObservableCollection<Cabinet>();
        public ObservableCollection<Cabinet> CabinetList
        {
            get { return _cabinetList; }
            set { SetProperty(ref _cabinetList, value); }
        }

        private ObservableCollection<CabinetViewModel> _cabsVM = new ObservableCollection<CabinetViewModel>();
        public ObservableCollection<CabinetViewModel> CabsVM
        {
            get { return _cabsVM; }
            set { SetProperty(ref _cabsVM, value); }
        }

        private ObservableCollection<RS485device> _devicesForProgramming = new ObservableCollection<RS485device>();
        public ObservableCollection<RS485device> DevicesForProgramming
        {
            get { return _devicesForProgramming; }
            set { SetProperty(ref _devicesForProgramming, value); }
        }

        private ObservableCollection<string> _availableComPorts = new ObservableCollection<string>();
        

        public ObservableCollection<string>  AvailableComPorts
        {
            get { return _availableComPorts; }
            set { SetProperty(ref _availableComPorts, value); }
        }

        private IEventAggregator _ea;
        private IDataRepositoryService _dataRepositoryService;
        private ISerialSender _serialSender;
        private Dispatcher _dispatcher;

        public ViewRS485ViewModel(IRegionManager regionManager,
                                  //IMessageService messageService,
                                  ISerialSender serialSender,
                                  IDataRepositoryService dataRepositoryService,
                                  IEventAggregator ea) : base(regionManager)
        {
            _ea = ea;
            _dataRepositoryService = dataRepositoryService;
            _serialSender = serialSender;
            _ea.GetEvent<MessageSentEvent>().Subscribe(MessageReceived);

            _dispatcher = Dispatcher.CurrentDispatcher;

            AvailableComPorts = _serialSender.GetAvailableCOMPorts();// Заполняем коллецию с доступными COM-портами
            /*
            StartCommand = new DelegateCommand(async () => await StartCommandExecuteAsync(), StartCommandCanExecute)
                .ObservesProperty(() => CanExec)
                .ObservesProperty(() => CurrentPort);
            */

            StartCommand = new DelegateCommand(async () => await StartCommandExecuteAsync(), StartCommandCanExecute)
                .ObservesProperty(() => CurrentPort);

            Title = "RS485";
            //Message = "View RS485 from your Prism Module";
        }

        private bool StartCommandCanExecute()
        {
            if (DevicesForProgramming.Count > 0 && CurrentPort != null && SerialTextBox != null) return true;
            return false;
        }

        private Task StartCommandExecuteAsync()
        {
            return Task.Run(() => DownloadLoop());
        }

        private void DownloadLoop()
        {
            SerialPort sp = _serialSender.GetSerialPortObjectRef();

            sp.PortName = CurrentPort;
            // check port is open or not
            if (sp.IsOpen == true)
            {
                Console.WriteLine("Port is open");
            }
            // set the port parameters
            sp.BaudRate = 9600;

            sp.DtrEnable = false;
            sp.Handshake = Handshake.None;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.DataBits = 8;

            RS485device device;
            if (DeviceCounter < DevicesForProgramming.Count)
            {
                device = DevicesForProgramming[DeviceCounter];
                if (device.Serial == null)//исключаем приборы уже имеющие серийник (они уже были сконфигурированны)
                {
                    _dispatcher.BeginInvoke(new Action(() =>
                    {
                        CurrentDeviceModel = device.Model;
                    }));
                    
                    device.Serial = SerialTextBox;
                    if (_serialSender.ChangeDeviceAddress(127, Convert.ToByte(device.AddressRS485)))
                    {
                        _dataRepositoryService.SaveDevice(device);
                        SerialTextBox = null;// Очищаем строку ввода серийника для ввода следующего
                        DeviceCounter++;
                    }
                    // Обновляем всю коллекцию в UI целиком
                    _dispatcher.BeginInvoke(new Action(() =>
                    {
                        CollectionViewSource.GetDefaultView(DevicesForProgramming).Refresh();
                    }));
                    if(DeviceCounter > DevicesForProgramming.Count) MessageBox.Show("Alles!");
                }
            }
        }

        private void MessageReceived(Message message)
        {
            if (message.ActionCode == MessageSentEvent.RepositoryUpdated)
            {
                CabinetList.Clear();
                foreach (Cabinet item in _dataRepositoryService.GetCabinetsWithDevices<RS485device>())
                {
                    CabinetList.Add(item);
                    CabsVM.Add(new CabinetViewModel(item, _ea));// Fill the TreeView with cabinets
                }
            }
            if (message.ActionCode == MessageSentEvent.UserSelectedItemInTreeView)
            {
                object dev = message.AttachedObject;
                if (IsCheckedByCabinets)
                {
                    if (dev.GetType() == typeof(RS485device)) // Юзер кликнул на прибор в дереве
                    {
                        DevicesForProgramming.Clear();
                        DevicesForProgramming.Add((RS485device)message.AttachedObject);
                    }
                    if (dev.GetType() == typeof(Cabinet)) //Юзер кликнул на шкаф в дереве
                    {
                        DevicesForProgramming.Clear();
                        Cabinet cab = (Cabinet)message.AttachedObject;
                        foreach (RS485device item in cab.GetDevicesList<RS485device>())
                        {
                            DevicesForProgramming.Add(item);
                        }
                    }
                }
                if (IsCheckedComplexVerification)
                {
                    if (dev.GetType() == typeof(Cabinet)) //Юзер кликнул на шкаф в дереве
                    {
                        DevicesForProgramming.Clear();
                        Cabinet cab = (Cabinet)message.AttachedObject;
                        foreach (RS485device item in cab.GetDevicesList<RS485device>())
                        {
                            DevicesForProgramming.Add(item);
                        }
                        
                    }
                }
            }
        }

        
    }
}
