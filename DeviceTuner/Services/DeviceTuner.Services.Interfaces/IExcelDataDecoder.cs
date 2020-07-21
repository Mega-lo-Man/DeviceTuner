using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Services.Interfaces
{
    public interface IExcelDataDecoder
    {
        List<NetworkDevice> GetNetworkDevices(string excelFileFullPath);
        bool SaveDevice<T>(T arg) where T : SimplestСomponent;
    }
}
