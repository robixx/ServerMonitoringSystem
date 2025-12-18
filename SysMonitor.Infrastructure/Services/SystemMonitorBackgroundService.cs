using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SysMonitor.Application.IInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SysMonitor.Infrastructure.Services
{
    public class SystemMonitorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public SystemMonitorBackgroundService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _provider.CreateScope())
                {
                    var monitorService = scope.ServiceProvider.GetRequiredService<ISystemMonitorService>();
                    await monitorService.StartMonitoringAsync(stoppingToken);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
