using DeviceTuner.Services.Interfaces;
using System.Collections.Generic;
using DeviceTuner.SharedDataModel;
using DeviceTuner.Core;
using Prism.Events;
using System;
using System.Windows.Data;
using System.Xml.Xsl;

namespace DeviceTuner.Services
{
    public class DataRepositoryService : IDataRepositoryService
    {
        private List<EthernetSwitch> switchDevs = new List<EthernetSwitch>();

        private IEventAggregator _ea;
        private IExcelDataDecoder _excelDataDecoder;
        
        private int _dataProviderType = 1;

        public DataRepositoryService(IEventAggregator ea, IExcelDataDecoder excelDataDecoder)
        {
            _ea = ea;
            _excelDataDecoder = excelDataDecoder;
        }

        private bool SaveSwitchDevice(EthernetSwitch ethernetSwitch)
        {
            //throw new NotImplementedException();
            if (_dataProviderType == 1) // Если источник данных - таблица Excel
            {
                _excelDataDecoder.SaveDevice(ethernetSwitch);
                return true;
            }
            return false;
        }

        public void SetDevices(int DataProviderType, string FullPathToData)
        {
            _dataProviderType = DataProviderType;
            string _fullPathToData = FullPathToData;
            switchDevs.Clear();
            switch (_dataProviderType)
            {
                case 1: switchDevs = _excelDataDecoder.GetSwitchDevices(_fullPathToData); break;
            }
            //Сообщаем об обновлении данных в репозитории
            _ea.GetEvent<MessageSentEvent>().Publish(Tuple.Create(MessageSentEvent.RepositoryUpdated, "1"));
        }

        public bool SaveDevice<T>(T arg) where T : SimplestСomponent
        {
            object someDevice = arg;
            if (typeof(T) == typeof(EthernetSwitch)) return SaveSwitchDevice((EthernetSwitch)someDevice);
            return false;
        }

        public IList<T> GetDevices<T>() where T : SimplestСomponent
        {
            if (typeof(T) == typeof(EthernetSwitch)) return (IList<T>)switchDevs;

            return null;
        }
    }
}
