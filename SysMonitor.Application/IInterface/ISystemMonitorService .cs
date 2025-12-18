using SysMonitor.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application.IInterface
{
    public interface ISystemMonitorService
    {
        Task<SystemStatusDto> GetSystemStatusAsync();
        Task StartMonitoringAsync(CancellationToken cancellationToken);
    }
}
