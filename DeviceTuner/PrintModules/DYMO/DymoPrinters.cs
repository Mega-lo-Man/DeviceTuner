using DymoSDK.Implementations;
using DeviceTuner.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PrintModules.DYMO
{
    public class DymoPrinters : IPrintService
    {
        private IEnumerable<DymoSDK.Interfaces.IPrinter> Printers;
        List<DymoSDK.Interfaces.ILabelObject> LabelObjects;
        DymoLabel dymoSDKLabel;
        string SelectedRoll;

        public DymoPrinters()
        {
            dymoSDKLabel = new DymoLabel();
            dymoSDKLabel.LoadLabelFromFilePath("label1.label");
            Printers = DymoPrinter.Instance.GetPrinters();
            TwinTurboRolls = new List<string>() { "Auto", "Left", "Right" };
        }

        public List<string> TwinTurboRolls { get; set; }

        public List<string> GetAvailablePrinters()
        {
            List<string> PrintersNameList = new List<string>();
            
            
            foreach (DymoSDK.Interfaces.IPrinter printer in Printers)
            {
                PrintersNameList.Add(printer.ToString());
            }
            return PrintersNameList;
        }

        public bool PrintLabel(string PrinterName, int LabelType, Dictionary<string, string> LabelDict)
        {
            
            if (PrinterName != null && Printers != null)
            {
                int copies = 1;

                DymoSDK.Interfaces.IPrinter SelectedPrinter = GetPrinterByName(PrinterName);

                if (SelectedPrinter != null)
                {
                    bool barcodeOrGraphsquality = false;

                    //Default quality is TEXT
                    //Setting barcodeGraphsQuality will improve printing quality being easier to read Barcode or QRcode objects
                    if (ContainsBarcodeOrGraphObjects())
                    {
                        barcodeOrGraphsquality = true;
                    }

                    //Send to print
                    if (SelectedPrinter.Name.Contains("Twin Turbo"))
                    {
                        //0: Auto, 1: Left roll, 2: Right roll
                        int rollSel = SelectedRoll == "Auto" ? 0 : SelectedRoll == "Left" ? 1 : 2;

                        DymoPrinter.Instance.PrintLabel(dymoSDKLabel, SelectedPrinter.Name, copies, rollSelected: rollSel, barcodeGraphsQuality: barcodeOrGraphsquality);
                    }
                    else
                        DymoPrinter.Instance.PrintLabel(dymoSDKLabel, SelectedPrinter.Name, copies, barcodeGraphsQuality: barcodeOrGraphsquality);

                }
            }
            return true;
        }

        private bool ContainsBarcodeOrGraphObjects()
        {
            foreach (var obj in LabelObjects)
            {
                if (obj.Type == DymoSDK.Interfaces.TypeObject.BarcodeObject || obj.Type == DymoSDK.Interfaces.TypeObject.QRCodeObject)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// // Search printer object by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private DymoSDK.Interfaces.IPrinter GetPrinterByName(string Name)
        {
            if (Printers != null)
            {
                foreach (DymoSDK.Interfaces.IPrinter printer in Printers)
                {
                    if (Name.Equals(printer.ToString()))
                    {
                        return printer;
                    }
                }
            }
            return null;
        }
    }
}
