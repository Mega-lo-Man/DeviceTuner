using System;
using System.Collections.Generic;
using System.Text;
using DeviceTuner.SharedDataModel;

namespace DeviceTuner.Services.Interfaces
{
    public interface IDataRepositoryService
    {
        /// <summary>
        /// Установить список устройств (найти в источнике данных все устройства)
        /// </summary>
        void SetDevices(int DataProviderType, string FullPathToData);

        /// <summary>
        /// Получить список устройств для настройки
        /// </summary>
        /// <typeparam name="T">тип устройства унаследованный от SimplestComponent</typeparam>
        /// <returns>Список устройств типа Т</returns>
        IList<T> GetDevices<T>() where T : SimplestСomponent;

        /// <summary>
        /// Записать свойства дивайса в таблицу Excel или базу данных
        /// </summary>
        /// <typeparam name="T">тип устройства унаследованный от SimplestComponent</typeparam>
        /// <param name="device">экземпляр устройства</param>
        /// <returns>true - есди запись удалась, false - в противном случае</returns>
        bool SaveDevice<T>(T device) where T : SimplestСomponent;
    }
}
