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
        private List<Cabinet> cabinetsLst = new List<Cabinet>();

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
            
            cabinetsLst.Clear();
            switch (_dataProviderType)
            {
                case 1:
                    cabinetsLst = _excelDataDecoder.GetCabinetsFromExcel(_fullPathToData);
                    break;
            }
            //Сообщаем всем об обновлении данных в репозитории
            _ea.GetEvent<MessageSentEvent>().Publish(new Message {
                ActionCode = MessageSentEvent.RepositoryUpdated
            });
        }

        public bool SaveDevice<T>(T arg) where T : SimplestСomponent
        {
            object someDevice = arg;
            if (typeof(T) == typeof(EthernetSwitch)) return SaveSwitchDevice((EthernetSwitch)someDevice);
            return false;
        }

        public IList<Cabinet> GetDevices<T>() where T : SimplestСomponent
        {
            // Надо удалить все шкафы которые не содержат приборов типа Т
            List<Cabinet> CabinetsWithdevs = new List<Cabinet>();
            foreach (Cabinet cabinet in cabinetsLst)
            {
                List<T> devicesList = (List<T>)cabinet.GetDevicesList<T>();
                if (devicesList.Count > 0)
                {
                    Cabinet newCabinet = new Cabinet
                    {
                        Designation = cabinet.Designation,
                        DeviceType = cabinet.DeviceType
                    };

                    foreach (T item in devicesList)
                    {
                        newCabinet.AddItem(item);
                    }
                    CabinetsWithdevs.Add(newCabinet);
                }
            }
            return CabinetsWithdevs;
        }
    }
}
