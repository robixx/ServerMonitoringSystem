using Microsoft.AspNetCore.SignalR;
using SysMonitor.Application;
using SysMonitor.Application.IInterface;
using SysMonitor.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;


using System.Text;

namespace SysMonitor.Infrastructure.Services
{
    public class SystemMonitorService : ISystemMonitorService
    {
        private readonly IHubContext<SystemMonitorHub> _hubContext;

        public SystemMonitorService(IHubContext<SystemMonitorHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<SystemStatusDto> GetSystemStatusAsync()
        {
            return await Task.Run(() => CollectSystemStatus());
        }

        public async Task StartMonitoringAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var status = CollectSystemStatus();
                await _hubContext.Clients.All.SendAsync("ReceiveStatus", status, token);
                await Task.Delay(1000, token); // send every 1 second
            }
        }

        private SystemStatusDto CollectSystemStatus()
        {
            var (totalMemory, usedMemory) = GetMemoryInfo();
            var cpuUsage = GetCpuUsage();

            var drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => new DriveInfoDto
                {
                    Name = d.Name,
                    TotalGB = d.TotalSize / 1_000_000_000,
                    FreeGB = d.AvailableFreeSpace / 1_000_000_000
                }).ToList();

            var processes = Process.GetProcesses()
                .Select(p => new ProcessInfoDto
                {
                    Name = p.ProcessName,
                    PID = p.Id,
                    CPUPercent = 0, // Advanced CPU calculation optional
                    MemoryMB = p.WorkingSet64 / 1024 / 1024
                }).ToList();

            var network = NetworkInterface.GetAllNetworkInterfaces()
                .Select(ni =>
                {
                    var stats = ni.GetIPv4Statistics();
                    return new NetworkDto
                    {
                        Name = ni.Name,
                        SentKB = stats.BytesSent / 1024,
                        ReceivedKB = stats.BytesReceived / 1024
                    };
                }).ToList();

            return new SystemStatusDto
            {
                CPU = cpuUsage,
                TotalRAM = totalMemory,
                UsedRAM = usedMemory,
                Drives = drives,
                RunningProcesses = processes,
                Network = network
            };
        }

        private float GetCpuUsage()
        {
            // Placeholder: use external library or OS-specific code if you need exact CPU %
            return new Random().Next(1, 100);
        }

        private (long total, long used) GetMemoryInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: read from /proc/meminfo alternative (cross-platform)
                return GetMemoryFromProc();
            }
            else
            {
                return GetMemoryFromProc();
            }
        }

        private (long total, long used) GetMemoryFromProc()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var lines = File.ReadAllLines("/proc/meminfo");
                var dict = lines.Select(l => l.Split(':'))
                                .ToDictionary(parts => parts[0], parts => long.Parse(parts[1].Trim().Split(' ')[0]) * 1024);
                long total = dict["MemTotal"];
                long free = dict["MemFree"] + dict.GetValueOrDefault("Buffers", 0) + dict.GetValueOrDefault("Cached", 0);
                return (total, total - free);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string output = RunBashCommand("vm_stat");
                long pageSize = 4096;
                long free = 0, used = 0;
                foreach (var line in output.Split('\n'))
                {
                    if (line.StartsWith("Pages free:"))
                        free = long.Parse(line.Split(':')[1].Trim().TrimEnd('.'));
                    if (line.StartsWith("Pages active:") || line.StartsWith("Pages wired down:") || line.StartsWith("Pages inactive:"))
                        used += long.Parse(line.Split(':')[1].Trim().TrimEnd('.'));
                }
                return ((free + used) * pageSize, used * pageSize);
            }
            else
            {
                // fallback
                long total = 16L * 1024 * 1024 * 1024; // 16GB
                long used = GC.GetTotalMemory(false);
                return (total, used);
            }
        }

        private string RunBashCommand(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}