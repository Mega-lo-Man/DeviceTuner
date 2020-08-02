using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTuner.SharedDataModel
{
    public class Cabinet : SimplestСomponent
    {
        private List<EthernetSwitch> _ethernetDevices = new List<EthernetSwitch>();
        private List<NetworkDevice> _networkDevices = new List<NetworkDevice>();
        private List<Device> _devices = new List<Device>();
        private List<SimplestСomponent> _simplestComponent = new List<SimplestСomponent>();

        private string _directoryPath;
        public string FilePath
        {
            get { return _directoryPath; }
            set { _directoryPath = value; }
        }

        public List<NetworkDevice> NetDevicesList
        {
            get { return _networkDevices; }
        }

        #region common
        public IList GetItemsList<T>() where T : SimplestСomponent
        {
            if (typeof(T) == typeof(EthernetSwitch)) return _ethernetDevices;
            if (typeof(T) == typeof(NetworkDevice)) return _networkDevices;
            if (typeof(T) == typeof(Device)) return _devices;
            if (typeof(T) == typeof(SimplestСomponent)) return _simplestComponent;
            return null;
        }
        public void AddItem<T>(T arg) where T : SimplestСomponent
        {
            object temp = arg;
            if (typeof(T) == typeof(EthernetSwitch)) { _networkDevices.Add((EthernetSwitch)temp); }
            if (typeof(T) == typeof(NetworkDevice)) { _networkDevices.Add((NetworkDevice)temp); }
            if (typeof(T) == typeof(Device)) { _devices.Add((Device)temp); }
            if (typeof(T) == typeof(SimplestСomponent)) { _simplestComponent.Add((SimplestСomponent)temp); }
        }
        #endregion
    }
}
