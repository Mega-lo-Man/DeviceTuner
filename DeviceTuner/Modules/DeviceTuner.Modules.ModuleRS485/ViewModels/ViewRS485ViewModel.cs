using DeviceTuner.Core;
using DeviceTuner.Core.Mvvm;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    public class ViewRS485ViewModel : RegionViewModelBase
    {
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

        private string _defaultRS485Address="127";
        public string DefaultRS485Address
        {
            get { return _defaultRS485Address; }
            set { SetProperty(ref _defaultRS485Address, value); }
        }

        private bool _isCheckedByCabinets = true;
        public bool IsCheckedByCabinets
        {
            get { return _isCheckedByCabinets; }
            set { SetProperty(ref _isCheckedByCabinets, value); }
        }

        private bool _isCheckedByArea = false;
        public bool IsCheckedByArea
        {
            get { return _isCheckedByArea; }
            set { SetProperty(ref _isCheckedByArea, value); }
        }

        private bool _isCheckedComplexVerification = false;
        public bool IsCheckedComplexVerification
        {
            get { return _isCheckedComplexVerification; }
            set { SetProperty(ref _isCheckedComplexVerification, value); }
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

        private IEventAggregator _ea;
        private IDataRepositoryService _dataRepositoryService;

        public ViewRS485ViewModel(IRegionManager regionManager,
                                  IMessageService messageService,
                                  IDataRepositoryService dataRepositoryService,
                                  IEventAggregator ea) : base(regionManager)
        {
            _ea = ea;
            _dataRepositoryService = dataRepositoryService;

            _ea.GetEvent<MessageSentEvent>().Subscribe(MessageReceived);

            StartCommand = new DelegateCommand(async () => await StartCommandExecuteAsync(), StartCommandCanExecute);

            Title = "RS485";
            //Message = "View RS485 from your Prism Module";
        }

        private bool StartCommandCanExecute()
        {
            if (DevicesForProgramming.Count > 0) return true;
            return false;
        }

        private Task StartCommandExecuteAsync()
        {
            throw new NotImplementedException();
        }

        private void MessageReceived(Message message)
        {
            if (message.ActionCode == MessageSentEvent.RepositoryUpdated)
            {
                CabinetList.Clear();
                foreach (Cabinet item in _dataRepositoryService.GetDevices<RS485device>())
                {
                    CabinetList.Add(item);
                    CabsVM.Add(new CabinetViewModel(item, _ea));// Fill the TreeView with cabinets
                }
            }
            if (message.ActionCode == MessageSentEvent.UserSelectedItemInTreeView)
            {
                object dev = message.AttachedObject;
                if (dev.GetType() == typeof(RS485device))
                {
                    DevicesForProgramming.Clear();
                    DevicesForProgramming.Add((RS485device)message.AttachedObject);
                }
                if (dev.GetType() == typeof(Cabinet))
                {
                    DevicesForProgramming.Clear();
                    Cabinet cab = (Cabinet)message.AttachedObject;
                    foreach(RS485device item in cab.GetDevicesList<RS485device>())
                    {
                        DevicesForProgramming.Add(item);
                    }
                }
            }
        }

        #region Commands
        public DelegateCommand StartCommand { get; private set; }
        #endregion
    }
}
