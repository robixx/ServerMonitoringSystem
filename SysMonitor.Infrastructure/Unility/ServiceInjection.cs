

using Microsoft.Extensions.DependencyInjection;
using SysMonitor.Application.IInterface;
using SysMonitor.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace SysMonitor.Infrastructure
{
    public static class ServiceInjection
    {
        public static void InjectService(this IServiceCollection services)
        {
            services.AddScoped<JwtConfig>();
            services.AddScoped<IAuth, AuthService>();
            services.AddSingleton<ISystemMonitorService, SystemMonitorService>();
            services.AddHostedService<SystemMonitorBackgroundService>();

        }
    }
}
