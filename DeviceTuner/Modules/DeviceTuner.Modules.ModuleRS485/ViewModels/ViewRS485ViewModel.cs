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

        private ObservableCollection<Cabinet> _cabinetList = new ObservableCollection<Cabinet>();
        public ObservableCollection<Cabinet> CabinetList
        {
            get { return _cabinetList; }
            set { SetProperty(ref _cabinetList, value); }
        }

        private ObservableCollection<CabinetViewModel> _cabs = new ObservableCollection<CabinetViewModel>();
        public ObservableCollection<CabinetViewModel> Cabs
        {
            get { return _cabs; }
            set { SetProperty(ref _cabs, value); }
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

            Title = "RS485";
            Message = "View RS485 from your Prism Module";
        }

        private void MessageReceived(Tuple<int, string> message)
        {
            if (message.Item1 == MessageSentEvent.RepositoryUpdated)
            {
                CabinetList.Clear();
                foreach (Cabinet item in _dataRepositoryService.GetDevices<RS485device>())
                {
                    CabinetList.Add(item);
                    CabinetViewModel cabinetViewModel = new CabinetViewModel(item);
                    //cabinetViewModel.Children();
                    Cabs.Add(new CabinetViewModel(item));
                }
            }
        }
    }
}
