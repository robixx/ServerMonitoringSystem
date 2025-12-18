using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application
{
    public class DriveInfoDto
    {
        public string? Name { get; set; }
        public long TotalGB { get; set; }
        public long FreeGB { get; set; }
    }
}
