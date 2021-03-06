﻿using DeviceTuner.SharedDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Services.Interfaces
{
    public interface ISender
    {
        
        /// <summary>
        /// Заливка конфига в коммутатор по протоколу Telnet или SSH
        /// </summary>
        /// <param name="networkDevice"></param>
        /// <returns>Объект типа NetworkDevices с заполненными полями (которые удалось выцепить из коммутатора)</returns>
        public NetworkDevice Send(NetworkDevice networkDevice, Dictionary<string, string> SettingsDict);
        /// <summary>
        /// Создание нового сетевого соединения
        /// </summary>
        /// <param name="IPaddress">IP адрес коммутатора</param>
        /// <param name="Port">Сетевой порт (22 для SSH-соединения , 23 для Telnet-соединения)</param>
        /// <param name="UserName">Имя учетной записи на коммутаторе (например, по умолчанию admin)</param>
        /// <param name="Password">Пароль от учетной записи на коммутаторе (например, по умолчанию admin)</param>
        /// <returns>True - соединение успешно создано, False - в противном случае</returns>
        public bool CreateConnection(string IPaddress, ushort Port, string Username, string Password);
        /// <summary>
        /// Завершение сетевого подключения
        /// </summary>
        /// <returns>True - соединение успешно разорвано, False - в противном случае</returns>
        public bool CloseConnection();
    }
}
