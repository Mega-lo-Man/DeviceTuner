using System;
using System.Collections.Generic;
using System.Text;
using DeviceTuner.SharedDataModel;

namespace DeviceTuner.Services.Interfaces
{
    public interface IDataRepositoryService
    {
        List<NetworkDevice> GetDevices();
        void SetDevices(List<NetworkDevice> devices);
    }
}
