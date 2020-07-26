using System;
using System.Collections.Generic;
using System.Text;
using DeviceTuner.SharedDataModel;

namespace DeviceTuner.Services.Interfaces
{
    public interface IDataRepositoryService
    {
        /// <summary>
        /// Тип поставщика данных (0 - Excel, 1 - SQL-база данных)
        /// </summary>
        int DataProviderType { get; set; }

        /// <summary>
        /// Полный путь к источнику данных (Excel-файл, адрес экземпляра базы данных)
        /// </summary>
        string FullPathToData { get; set; }

        /// <summary>
        /// Пулучить список коммутаторов
        /// </summary>
        /// <returns></returns>
        List<NetworkDevice> GetSwitchDevices();

        /// <summary>
        /// Установить список устройств
        /// </summary>
        void SetDevices();

        /// <summary>
        /// Записать свойства дивайса в таблицу Excel или базу данных
        /// </summary>
        /// <typeparam name="T">унаследованный от SimplestComponent</typeparam>
        /// <param name="device">экземпляр устройства</param>
        /// <returns>true - есди запись удалась, false - в противном случае</returns>
        bool SaveDevice<T>(T device) where T : SimplestСomponent;
    }
}
