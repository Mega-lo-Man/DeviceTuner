using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Core
{
    public class Message
    {
        public int ActionCode { get; set; }
        public string MessageString { get; set; }
        public object AttachedObject { get; set; }
    }
}
