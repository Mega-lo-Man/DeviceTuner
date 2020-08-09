using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Transactions;

namespace DeviceTuner.SharedDataModel
{
    public class Cabinet : SimplestСomponent
    {
        private List<object> objLst = new List<object>(); // крнтейнер для хранения всех дивайсов внутри шкафа

        public IList<object> GetAllDevicesList
        {
            get
            {
                ObservableCollection<object> childNodes = new ObservableCollection<object>();

                foreach (var item in objLst)
                    childNodes.Add(item);

                return childNodes;
            }
        }

        #region common
        public IList<T> GetDevicesList<T>() where T : SimplestСomponent
        {
            List<T> lst = new List<T>();
            foreach (var item in objLst)
            {
                if (item.GetType() == typeof(T))
                { 
                    lst.Add((T)item); 
                }
            }
            return lst;
        }

        public void AddItem<T>(T arg) where T : SimplestСomponent
        {
            objLst.Add(arg);
        }

        public void ClearItems()
        {
            objLst.Clear();
        }
        #endregion
    }
}
