using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using System.IO;
using System.Net;

namespace DeviceTuner.Services
{
    public class ExcelDataDecoder : IExcelDataDecoder
    {
        private int addressCol = 0; // Index of column that containing device addresses
        private int nameCol = 0;    // Index of column that containing device names
        private int serialCol = 0;  // Index of column that containing device serial number
        private int modelCol = 0;   // Index of column that containing device model
        private int CaptionRow = 1; //Table caption row index

        private string ColAddressCaption = "IP"; //Заголовок столбца с адресами
        private string ColNamesCaption = "Обозначение"; //Заголовок столбца с обозначениями приборов
        private string ColSerialCaption = "Серийный номер"; //Заголовок столбца с обозначениями приборов
        private string ColModelCaption = "Модель"; //Заголовок столбца с наименование модели прибора

        private ExcelPackage package;
        private FileInfo sourceFile;
        private ExcelWorksheet worksheet;
        int rows; // number of rows in the sheet
        int columns;//number of columns in the sheet

        public ExcelDataDecoder()
        {
            // Remove "IBM437 is not a supported encoding" error
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private int GetDeviceType(string DevName)
        {
            int devType = 0;
            if (DevName.Contains("MES3508")) devType = 1;
            if (DevName.Contains("MES2308")) devType = 1;
            if (DevName.Contains("MES2324")) devType = 1;
            if (DevName.Contains("2000-Ethernet")) devType = 2;
            return devType;
        }

        public List<NetworkDevice> GetSwitchDevices(string excelFileFullPath)
        {
            sourceFile = new FileInfo(excelFileFullPath);
            List<NetworkDevice> devices = new List<NetworkDevice>();

            package = new ExcelPackage(sourceFile);

            worksheet = package.Workbook.Worksheets["Адреса"];
            /*
            worksheet.Cells[1, 1].Value = "tytytyty";
            worksheet.Cells["A2"].Value = "opopopop";
            */
            // get number of rows and columns in the sheet
            rows = worksheet.Dimension.Rows; // 20
            columns = worksheet.Dimension.Columns; // 7

            //Определяем в каких столбцах находятся обозначения приборов и их адреса
            FindColumnIndexesByHeader();
            
            for (int rowIndex = CaptionRow + 1; rowIndex <= rows; rowIndex++)
            {
                string devName = worksheet.Cells[rowIndex, nameCol].Value?.ToString();
                string devModel = worksheet.Cells[rowIndex, modelCol].Value?.ToString();
                string devAddr = worksheet.Cells[rowIndex, addressCol].Value?.ToString();
                string devSerial = worksheet.Cells[rowIndex, serialCol].Value?.ToString();
                //Чтобы попасть в список устройств для заливки конфига, дивайс должен иметь адрес, 
                //обозначение и отсутсвие серийника.
                if (devAddr != null && devName != null && devSerial == null)
                {
                    // Проверяем содержит ли строка адрес. Парсинг + три точки-разделителя в адресной строке (X.X.X.X)
                    if (IPAddress.TryParse(devAddr, out IPAddress parseAddress) && (devAddr.Split('.').Length - 1) == 3)
                    {
                        //Valid IP, with address containing the IP
                        switch (GetDeviceType(devModel))
                        {
                            case 1:
                                devices.Add(new NetworkDevice
                                {
                                    Designation = devName,
                                    AddressIP = parseAddress.ToString(),
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
            return devices;
        }

        private void FindColumnIndexesByHeader()
        {
            for (int colIndex = 1; colIndex <= columns; colIndex++)
            {
                string content = worksheet.Cells[CaptionRow, colIndex].Value?.ToString();
                if (content == ColNamesCaption) { nameCol = colIndex; }
                if (content == ColAddressCaption) { addressCol = colIndex; }
                if (content == ColSerialCaption) { serialCol = colIndex; }
                if (content == ColModelCaption) { modelCol = colIndex; }
            }
        }

        public bool SaveDevice<T>(T arg) where T : SimplestСomponent
        {
            object someDevice = arg;
            if (typeof(T) == typeof(NetworkDevice)) return SaveNetworkDevice((NetworkDevice)someDevice);
            return false;
        }

        private bool SaveNetworkDevice(NetworkDevice networkDevice)
        {
            //поиск в таблице строки которая содержит IP-адрес такой же как в networkDevice
            int? foundRow = SearchRowByCellValue(networkDevice.AddressIP, addressCol); 
            if (foundRow != null)
            {
                // записываем серийник коммутатора в графу "Серийный номер" напротив IP-адреса этого коммутатора
                worksheet.Cells[foundRow.Value, serialCol].Value = networkDevice.Serial;
                package.Save();
                return true;
            }
            return false;
        }

        // Поиск номера строки к которой относится только что сконфигурированный дивайс
        // searchValue - что ищем, column - столбец в котором ищем
        private int? SearchRowByCellValue(string searchValue, int column)
        {
            //Return first entry
            for (int rowCounter = CaptionRow + 2; rowCounter <= rows; rowCounter++)
            {
                if(searchValue.Equals(worksheet.Cells[rowCounter, column].Value?.ToString())) return rowCounter;
            }
            return null;
        }
    }
}
