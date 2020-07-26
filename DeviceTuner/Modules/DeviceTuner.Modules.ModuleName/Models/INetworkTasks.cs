using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Modules.ModuleSwitch.Models
{
    public interface INetworkTasks
    {
        public bool UploadConfigStateMachine(NetworkDevice nDevice, Dictionary<string, string> settings);
        public bool SendPing(string NewIPAddress);
    }
}
