using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using System.IO;

namespace DeviceTuner.Services
{
    public class ExcelDataDecoder : IExcelDataDecoder
    {
        private int addressCol = 0; // Index column that containing device addresses
        private int nameCol = 0;    // Index column that containing device names
        private int serialCol = 0;  // Index column that containing device serial number
        private int modelCol = 0;   // Index column that containing device model
        private int CaptionRow = 1; //Table caption index

        private string ColAddressCaption = "IP"; //Заголовок столбца с адресами
        private string ColNamesCaption = "Обозначение"; //Заголовок столбца с обозначениями приборов
        private string ColSerialCaption = "Серийный номер"; //Заголовок столбца с обозначениями приборов
        private string ColModelCaption = "Модель"; //Заголовок столбца с наименование модели прибора

        FileInfo sourceFile;
        ExcelWorksheet worksheet;
        int rows; // number of rows in the sheet
        int columns;//number of columns in the sheet

        private int GetDeviceType(string DevName)
        {
            int devType = 0;
            if (DevName.Contains("MES3508")) devType = 1;
            if (DevName.Contains("MES2308")) devType = 1;
            if (DevName.Contains("MES2324")) devType = 1;
            if (DevName.Contains("2000-Ethernet")) devType = 2;
            return devType;
        }

        public List<NetworkDevice> GetNetworkDevices(string excelFileFullPath)
        {
            sourceFile = new FileInfo(excelFileFullPath);
            List<NetworkDevice> devices = new List<NetworkDevice>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);// Remove "IBM437 is not a supported encoding" error

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(sourceFile))
            {
                worksheet = package.Workbook.Worksheets["Адреса"];
                /*
                worksheet.Cells[1, 1].Value = "tytytyty";
                worksheet.Cells["A2"].Value = "opopopop";
                */
                // get number of rows and columns in the sheet
                rows = worksheet.Dimension.Rows; // 20
                columns = worksheet.Dimension.Columns; // 7

                //Определяем в каких столбцах находятся обозначения приборов и их адреса
                for (int colIndex = 1; colIndex <= columns; colIndex++)
                {
                    string content = worksheet.Cells[CaptionRow, colIndex].Value?.ToString();
                    if (content == ColNamesCaption) { nameCol = colIndex; }
                    if (content == ColAddressCaption) { addressCol = colIndex; }
                    if (content == ColSerialCaption) { serialCol = colIndex; }
                    if (content == ColModelCaption) { modelCol = colIndex; }
                }
                //
                for (int rowIndex = CaptionRow + 1; rowIndex <= rows; rowIndex++)
                {
                    string devName = worksheet.Cells[rowIndex, nameCol].Value?.ToString();
                    string devModel = worksheet.Cells[rowIndex, modelCol].Value?.ToString();
                    string devAddr = worksheet.Cells[rowIndex, addressCol].Value?.ToString();
                    if (devAddr != null && devName != null)
                    {
                        // Проверяем содержит ли строка адрес. Парсинг + три точки-разделителя в адрессной строке (X.X.X.X)
                        if (System.Net.IPAddress.TryParse(devAddr, out System.Net.IPAddress parseAddress) && (devAddr.Split('.').Length - 1) == 3)
                        {
                            //Valid IP, with address containing the IP
                            switch (GetDeviceType(devModel))
                            {
                                case 1:
                                    devices.Add(new NetworkDevice
                                    {
                                        Designation = devName,
                                        AddressIP = parseAddress.ToString(),
                                        //ExcelRowIndex = rowIndex,
                                        //DownloadedSuccessfully = false
                                    });
                                    break;
                                case 2:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return devices;
        }
    }
}
