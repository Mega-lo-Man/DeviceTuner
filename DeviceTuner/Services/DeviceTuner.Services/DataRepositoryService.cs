using DeviceTuner.Services.Interfaces;
using System.Collections.Generic;
using DeviceTuner.SharedDataModel;
using DeviceTuner.Core;
using Prism.Events;
using System;
using System.Windows.Data;

namespace DeviceTuner.Services
{
    public class DataRepositoryService : IDataRepositoryService
    {
        private List<NetworkDevice> switchDevs = new List<NetworkDevice>();

        private IEventAggregator _ea;
        private IExcelDataDecoder _excelDataDecoder;
        
        #region Properties
        private int _dataProviderType = 1;
        public int DataProviderType
        {
            get { return _dataProviderType; }
            set { _dataProviderType = value; }
        }

        private string _fullPathToData = "";
        public string FullPathToData
        {
            get { return _fullPathToData; }
            set { _fullPathToData = value; }
        }
        #endregion Properties

        public DataRepositoryService(IEventAggregator ea, IExcelDataDecoder excelDataDecoder)
        {
            _ea = ea;
            _excelDataDecoder = excelDataDecoder;
        }

        public List<NetworkDevice> GetSwitchDevices()
        {
            if (_fullPathToData == "")
            {
                throw new Exception("Не установлен путь до источника данных!");
            }
            else
            {
                return switchDevs;
            }
        }

        private bool SaveSwitchDevice(NetworkDevice networkDevice)
        {
            //throw new NotImplementedException();
            if (DataProviderType == 1) // Если источник данных - таблица Excel
            {
                _excelDataDecoder.SaveDevice(networkDevice);
                return true;
            }
            return false;
        }

        public void SetDevices()
        {
            switchDevs.Clear();
            switchDevs = _excelDataDecoder.GetSwitchDevices(FullPathToData);
            //Сообщаем об обновлении данных в репозитории
            _ea.GetEvent<MessageSentEvent>().Publish(Tuple.Create(MessageSentEvent.RepositoryUpdated, "1"));
        }

        public bool SaveDevice<T>(T arg) where T : SimplestСomponent
        {
            object someDevice = arg;
            if (typeof(T) == typeof(NetworkDevice)) return SaveSwitchDevice((NetworkDevice)someDevice);
            return false;
        }
    }
}
