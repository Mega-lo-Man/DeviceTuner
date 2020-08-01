using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Services.Interfaces
{
    public interface IExcelDataDecoder
    {
        List<EthernetSwitch> GetSwitchDevices(string excelFileFullPath);
        bool SaveDevice<T>(T arg) where T : SimplestСomponent;
    }
}
