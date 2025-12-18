using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application
{
    public class NetworkDto
    {
        public string? Name { get; set; }
        public long SentKB { get; set; }
        public long ReceivedKB { get; set; }
    }
}
