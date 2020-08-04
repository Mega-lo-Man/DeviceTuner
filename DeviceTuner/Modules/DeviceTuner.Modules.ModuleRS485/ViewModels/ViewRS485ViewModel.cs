using DeviceTuner.Core.Mvvm;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Commands;
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

        private string _defaultRS485Address="123";
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

        #region IsExpanded

        private bool _isExpanded;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    SetProperty(ref _isExpanded, value);
                }
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        private bool _isSelected;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    SetProperty(ref _isSelected, value);
                }
            }
        }

        #endregion // IsSelected

        public ViewRS485ViewModel(IRegionManager regionManager, IMessageService messageService, IDataRepositoryService dataRepositoryService) :
            base(regionManager)
        {
            Title = "RS485";
            Message = "View RS485 from your Prism Module";

            
            Cabinet cab = new Cabinet();
            cab.Designation = "Шкапчик_1";
            
            NetworkDevice networkDevice = new NetworkDevice();
            networkDevice.Designation = "Прибор 1";
            cab.AddItem(networkDevice);

            Device device = new Device();
            device.Designation = "Dev 1";
            cab.AddItem(device);

            Device device2 = new Device();
            device2.Designation = "Dev 2";
            cab.AddItem(device2);

            CabinetList.Add(cab);

            Cabinet cab1 = new Cabinet();
            cab1.Designation = "Шкапчик_2";
            NetworkDevice networkDevice1 = new NetworkDevice();
            networkDevice1.Designation = "Прибор 2";
            cab1.AddItem(networkDevice1);
            CabinetList.Add(cab1);
        }
    }
}
