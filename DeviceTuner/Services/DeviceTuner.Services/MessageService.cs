﻿using DeviceTuner.Services.Interfaces;
using System;

namespace DeviceTuner.Services
{
    public class MessageService : IMessageService
    {
        public event Action<string> DataArrived;

        public string GetMessage()
        {
            return "Hello from the Message Service";
        }

        public void SendMessage(string sms)
        {
            throw new NotImplementedException();
        }
    }
}
