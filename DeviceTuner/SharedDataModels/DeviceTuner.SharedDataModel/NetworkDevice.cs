using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTuner.SharedDataModel
{
    public class NetworkDevice : Device
    {
        /// <summary>
        /// Адрес прибора на линии RS-485 ("23")
        /// </summary>
        private int? _address_RS485;
        public int? AddressRS485
        {
            get { return _address_RS485; }
            set { if (value > 0 && value < 127) _address_RS485 = value; }
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
        /// Адрес прибора на линии RS-232 ("24")
        /// </summary>
        private int? _address_RS232;
        public int? AddressRS232
        {
            get { return _address_RS232; }
            set { _address_RS232 = value; }
        }
    }
}
