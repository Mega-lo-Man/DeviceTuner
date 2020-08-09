using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.SharedDataModel
{
    public class RS232device : RS485device
    {
        /// <summary>
        /// Адрес прибора на линии RS-232 ("24")
        /// </summary>
        private int? _address_RS232;
        public int? AddressRS232
        {
            get { return _address_RS232; }
            set { if (value > 0 && value < 127) _address_RS232 = value; }
        }

        /// <summary>
        /// IP адрес прибора ("192.168.2.12")
        /// </summary>
        private string _address_IP;
        public string AddressIP
        {
            get { return _address_IP; }
            set { _address_IP = value; }
        }

        /// <summary>
        /// MAC-адрес прибора
        /// </summary>
        private string _macAddress;
        public string MACaddress
        {
            get { return _macAddress; }
            set { if (value.Length <= 17 && value.Length >= 12) _macAddress = value; }
        }
    }
}
