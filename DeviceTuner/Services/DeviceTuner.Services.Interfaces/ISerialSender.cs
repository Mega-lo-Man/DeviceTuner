﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO.Ports;
using System.Text;

namespace DeviceTuner.Modules.ModuleRS485.Models
{
    public interface ISerialSender
    {
        public SerialPort GetSerialPortObjectRef();

        /// <summary>
        /// Получить коллекцию установленных в системе COM-портов
        /// </summary>
        /// <returns>коллекцию названий доступных COM-портов (COM1, COM2, COM3, ...)</returns>
        public ObservableCollection<string> GetAvailableCOMPorts();

        /// <summary>
        /// Смена адреса устройства подключенного на линию RS485.
        /// </summary>
        /// <param name="deviceAddress">Текущий адрес</param>
        /// <param name="newDeviceAddress">Новый адрес</param>
        /// <returns>true если смена адреса прошла успешно, в противном случае - false</returns>
        public bool ChangeDeviceAddress(byte deviceAddress, byte newDeviceAddress);

        /// <summary>
        /// Поиск всех устройств на линии RS485.
        /// </summary>
        /// <param name="comPort">Номер COM-порта для опроса</param>
        /// <returns>список адресов ответивших устройств</returns>
        public List<byte> SearchOnlineDevices();

        /// <summary>
        /// Запрос модели устройства
        /// </summary>
        /// <param name="comPort">Номер COM-порта на которой находится устройство</param>
        /// <param name="deviceAddress">Адрес устройства</param>
        /// <returns>строку содержащую название модели</returns>
        public string GetDeviceModel(byte deviceAddress);

        /// <summary>
        /// "Пинг" устройства на линии RS485
        /// </summary>
        /// <param name="comPort">Номер COM-порта на которой находится устройство</param>
        /// <param name="deviceAddress">Адрес пингуемого устройства</param>
        /// <returns>true - прибор отвечает по указанному адресу, false - нет ответа</returns>
        public bool IsDeviceOnline(byte deviceAddress);
    }
}