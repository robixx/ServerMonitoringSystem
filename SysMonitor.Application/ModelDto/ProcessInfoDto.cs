using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application.ModelDto
{
    public class ProcessInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public int PID { get; set; }
        public float CPUPercent { get; set; }
        public long MemoryMB { get; set; }
    }
}
