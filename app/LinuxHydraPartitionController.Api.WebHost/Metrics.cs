using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using LinuxHydraPartitionController.Api.WebHost.Models;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Metrics
    {
        private readonly ILogger<Metrics> _logger;
        //private static ProcessStartInfo _metricsProcessStartInfo;

        public Metrics(ILogger<Metrics> logger)
        {
            _logger = logger;
        }
        public MachineMetrics GetMetrics()
        {
            var manageProcess = new ManageProcess(_logger);
            MachineMetrics _metrics = new();
            ProcessStartInfo _metricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/nproc;/usr/bin/cat /proc/loadavg|/usr/bin/awk '{print $1\\\"\\n\\\"$2\\\"\\n\\\"$3}'");
            string[] cpuLines = manageProcess.Execute(_metricsProcessStartInfo).Split("\n");
            _metrics.CPU.Cores = int.Parse(cpuLines[0]);
            _metrics.CPU.Load1min = float.Parse(cpuLines[1]);
            _metrics.CPU.Load5min = float.Parse(cpuLines[2]);
            _metrics.CPU.Load15min = float.Parse(cpuLines[3]);
            _logger.LogInformation($"CPU Line 0 and 3: {cpuLines[0]} {cpuLines[3]}");
            _metricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/free -m|/usr/bin/head -2|/usr/bin/tail -n +2|/usr/bin/awk '{print $2\\\"\\n\\\"$3\\\"\\n\\\"$4\\\"\\n\\\"$6\\\"\\n\\\"$7}'");
            string[] memlines = manageProcess.Execute(_metricsProcessStartInfo).Split("\n");
            _metrics.MemoryInMB.Total = int.Parse(memlines[0]);
            _metrics.MemoryInMB.Used = int.Parse(memlines[1]);
            _metrics.MemoryInMB.Free = int.Parse(memlines[2]);
            _metrics.MemoryInMB.Buffers = int.Parse(memlines[3]);
            _metrics.MemoryInMB.Available = int.Parse(memlines[4]);
            _metricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/cat /proc/uptime|/usr/bin/awk '{print int($1)}'");
            _metrics.UptimeInSeconds.Uptime = int.Parse(manageProcess.Execute(_metricsProcessStartInfo));
            return _metrics;
        }
    }
}
