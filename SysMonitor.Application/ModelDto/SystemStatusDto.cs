using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application.ModelDto
{
    public class SystemStatusDto
    {
        public float CPU { get; set; }
        public long UsedRAM { get; set; }
        public long TotalRAM { get; set; }
        public List<DriveInfoDto> Drives { get; set; } = new();
        public List<ProcessInfoDto> RunningProcesses { get; set; } = new();
        public List<NetworkDto> Network { get; set; }=new();
    }
}
