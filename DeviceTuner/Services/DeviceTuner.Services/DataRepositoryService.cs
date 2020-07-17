using DeviceTuner.Services.Interfaces;
using System.Collections.Generic;
using DeviceTuner.SharedDataModel;
using DeviceTuner.Core;
using Prism.Events;
using System;

namespace DeviceTuner.Services
{
    public class DataRepositoryService : IDataRepositoryService
    {
        private List<NetworkDevice> tempDevs = new List<NetworkDevice>();

        IEventAggregator _ea;

        public DataRepositoryService(IEventAggregator ea)
        {
            _ea = ea;
        }

        public List<NetworkDevice> GetDevices()
        {
            return tempDevs;
        }

        public void SetDevices(List<NetworkDevice> devices)
        {
            tempDevs.Clear();
            foreach (NetworkDevice item in devices)
            {
                tempDevs.Add(item);
            }
            _ea.GetEvent<MessageSentEvent>().Publish(Tuple.Create(MessageSentEvent.RepositoryUpdated, "1"));
        }
    }
}
