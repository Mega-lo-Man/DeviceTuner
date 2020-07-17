using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTuner.SharedDataModel
{
    public class Device : SimplestСomponent
    {        
        /// <summary>
        /// Серийный номер прибора ("456426")
        /// </summary>
        private string _serial;
        public string Serial
        {
            get { return _serial; }
            set { _serial = value; }
        }
        /// <summary>
        /// Название площадки на которой находится шкаф с этим прибором ("КС "Невинномысская"")
        /// </summary>
        private string _area;
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }
        /// <summary>
        /// Наименование шкафа в котором находится этот прибор
        /// </summary>
        private string _cabinet;
        public string Cabinet
        {
            get { return _cabinet; }
            set { _cabinet = value; }
        }
    }
}
