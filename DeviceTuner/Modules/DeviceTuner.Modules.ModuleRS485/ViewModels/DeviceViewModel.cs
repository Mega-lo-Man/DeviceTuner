using DeviceTuner.Core;
using DeviceTuner.SharedDataModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    class DeviceViewModel : TreeViewItemViewModel
    {
        private RS485device _device;
        private IEventAggregator _ea;

        public DeviceViewModel(RS485device device, CabinetViewModel cabinetParent, IEventAggregator ea) 
            : base(cabinetParent, false)
        {
            _ea = ea;
            _device = device;
        }

        public string GetDeviceDesignation
        {
            get { return _device.Designation; }
        }

        protected override void OnSelectedItemChanged()
        {
            //base.OnSelectedItemChanged();
            //Сообщаем всем об том, что юзер кликнул на объекте в дереве.
            //Объект на который юзер кликнул передаётся через свойство AttachedObject
            _ea.GetEvent<MessageSentEvent>().Publish(new Message
            {
                ActionCode = MessageSentEvent.UserSelectedItemInTreeView,
                AttachedObject = _device
            });
        }
    }
}
