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
        /// Получить список шкафов с приборами заданного типа T для настройки
        /// </summary>
        /// <typeparam name="T">тип прибора унаследованный от SimplestComponent</typeparam>
        /// <returns>Список шкафов с приборами типа Т</returns>
        IList<Cabinet> GetCabinetsWithDevices<T>() where T : SimplestСomponent;

        /// <summary>
        /// Получить список всех приборов типа T во всех шкафах
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<T> GetAllDevices<T>() where T : SimplestСomponent;

        /// <summary>
        /// Записать свойства прибора в таблицу Excel или базу данных
        /// </summary>
        /// <typeparam name="T">тип прибора унаследованный от SimplestComponent</typeparam>
        /// <param name="device">экземпляр прибора</param>
        /// <returns>true - есди запись удалась, false - в противном случае</returns>
        bool SaveDevice<T>(T device) where T : SimplestСomponent;
    }
}
