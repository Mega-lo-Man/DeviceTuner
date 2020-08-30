using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Modules.ModuleSwitch.Models
{
    public interface INetworkTasks
    {
        public bool UploadConfigStateMachine(EthernetSwitch nDevice, Dictionary<string, string> settings);
        public bool SendPing(string IPAddress);
        public bool SendMultiplePing(string IPAddress, int NumberOfRepetitions);
    }
}
