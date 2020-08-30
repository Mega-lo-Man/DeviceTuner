using DeviceTuner.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Services
{
    public class PrintService : IPrintService
    {
        public List<string> GetAvailablePrinters()
        {
            throw new NotImplementedException();
        }

        public bool PrintLabel(string PrinterName, int LabelType, Dictionary<string, string> LabelDict)
        {
            throw new NotImplementedException();
        }
    }
}
