using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    public class CabinetViewModel : TreeViewItemViewModel
    {
        private Cabinet _cabinet;
        public CabinetViewModel(Cabinet cabinet) : base(null, true)
        {
            _cabinet = cabinet;
        }

        public String GetCabinetDesignation
        {
            get { return _cabinet.Designation; }
        }

        protected override void LoadChildren()
        {
            foreach (RS485device device in _cabinet.GetDevicesList<RS485device>())
            {
                Children.Add(new DeviceViewModel(device, this));
            }
        }
    }
}
