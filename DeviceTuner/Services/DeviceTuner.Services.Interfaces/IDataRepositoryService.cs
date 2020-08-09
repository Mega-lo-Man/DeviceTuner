using System;
using System.Collections.Generic;
using System.Text;
using DeviceTuner.SharedDataModel;

namespace DeviceTuner.Services.Interfaces
{
    public interface IDataRepositoryService
    {
        /// <summary>
        /// Установить список приборов (найти в источнике данных все приборы)
        /// </summary>
        void SetDevices(int DataProviderType, string FullPathToData);

        /// <summary>
        /// Получить список приборов для настройки
        /// </summary>
        /// <typeparam name="T">тип прибора унаследованный от SimplestComponent</typeparam>
        /// <returns>Список шкафов с приборами типа Т</returns>
        IList<Cabinet> GetDevices<T>() where T : SimplestСomponent;

        /// <summary>
        /// Записать свойства прибора в таблицу Excel или базу данных
        /// </summary>
        /// <typeparam name="T">тип прибора унаследованный от SimplestComponent</typeparam>
        /// <param name="device">экземпляр прибора</param>
        /// <returns>true - есди запись удалась, false - в противном случае</returns>
        bool SaveDevice<T>(T device) where T : SimplestСomponent;
    }
}
