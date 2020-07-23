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
        private List<NetworkDevice> tempDevs = new List<NetworkDevice>();

        private IEventAggregator _ea;
        private IExcelDataDecoder _excelDataDecoder;
        private enum dataSource { Excel, ODBC } 
        private int DataSourceType = 1; //Excel

        public DataRepositoryService(IEventAggregator ea, IExcelDataDecoder excelDataDecoder)
        {
            _ea = ea;
            _excelDataDecoder = excelDataDecoder;
        }

        public List<NetworkDevice> GetDevices()
        {
            return tempDevs;
        }

        public bool SaveDevice(NetworkDevice networkDevice)
        {
            //throw new NotImplementedException();
            if (DataSourceType == 1) // Если источник данных - таблица Excel
            {
                _excelDataDecoder.SaveDevice(networkDevice);
                return true;
            }
            return false;
        }

        public void SetDevices(List<NetworkDevice> devices)
        {
            tempDevs.Clear();
            foreach (NetworkDevice item in devices)
            {
                tempDevs.Add(item);
            }
            _ea.GetEvent<MessageSentEvent>().Publish(Tuple.Create(MessageSentEvent.RepositoryUpdated, "1"));
        }
    }
}
