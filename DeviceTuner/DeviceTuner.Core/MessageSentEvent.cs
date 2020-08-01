using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Core
{
    public class MessageSentEvent : PubSubEvent<Tuple<int, string>>
    {
        public static readonly int RepositoryUpdated = 1;
        public static readonly int StringToConsole = 2;
        public static readonly int NeedOfUserAction = 3;
    }
}
