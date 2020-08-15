using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    class DeviceViewModel : TreeViewItemViewModel
    {
        private RS485device _device;

        public DeviceViewModel(RS485device device, CabinetViewModel cabinetParent) : base(cabinetParent, false)
        {
            _device = device;
        }

        public string GetDeviceDesignation
        {
            get { return _device.Designation; }
        }
    }
}
