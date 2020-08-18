using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Modules.ModuleRS485.Models
{
    public interface IRS485Task
    {
        /// <summary>
        /// Смена адреса устройства подключенного на линию RS485.
        /// </summary>
        /// <param name="deviceAddress">Текущий адрес</param>
        /// <param name="newDeviceAddress">Новый адрес</param>
        /// <returns>true если смена адреса прошла успешно, в противном случае - false</returns>
        public bool ChangeDeviceAddress(int comPort, int deviceAddress, int newDeviceAddress);

        /// <summary>
        /// Поиск всех устройств на линии RS485.
        /// </summary>
        /// <param name="comPort">Номер COM-порта для опроса</param>
        /// <returns>список адресов ответивших устройств</returns>
        public List<int> SearchOnlineDevices(int comPort);

        /// <summary>
        /// Запрос модели устройства
        /// </summary>
        /// <param name="comPort">Номер COM-порта на которой находится устройство</param>
        /// <param name="deviceAddress">Адрес устройства</param>
        /// <returns>строку содержащую название модели</returns>
        public string GetDeviceModel(int comPort, int deviceAddress);

        /// <summary>
        /// "Пинг" устройства на линии RS485
        /// </summary>
        /// <param name="comPort">Номер COM-порта на которой находится устройство</param>
        /// <param name="deviceAddress">Адрес пингуемого устройства</param>
        /// <returns></returns>
        public bool IsDeviceOnline(int comPort, int deviceAddress);
    }
}
