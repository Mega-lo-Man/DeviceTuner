using DeviceTuner.Core;
using DeviceTuner.SharedDataModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    public class CabinetViewModel : TreeViewItemViewModel
    {
        private Cabinet _cabinet;
        private IEventAggregator _ea;
        public CabinetViewModel(Cabinet cabinet, IEventAggregator ea) : base(null, true)
        {
            _ea = ea;
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
                Children.Add(new DeviceViewModel(device, this, _ea));
            }
        }

        protected override void OnSelectedItemChanged()
        {
            //base.OnSelectedItemChanged();
            //Сообщаем всем об том, что юзер кликнул на объекте в дереве.
            //Объект на который юзер кликнул передаётся через свойство AttachedObject
            _ea.GetEvent<MessageSentEvent>().Publish(new Message
            {
                ActionCode = MessageSentEvent.UserSelectedItemInTreeView,
                AttachedObject = _cabinet
            });
        }       
    }
}
