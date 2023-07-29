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
        private MachineMetrics machineMetrics = new MachineMetrics();
        private CPU cpu { get; set; }
        private MemoryInMB memory { get; set; }
        private UptimeInSeconds uptime { get; set; }

        private readonly ILogger<Metrics> _logger;

        public Metrics(ILogger<Metrics> logger)
        {
            _logger = logger;
        }
        public MachineMetrics GetMetrics()
        {
            var manageProcess = new ManageProcess(_logger);
            ProcessStartInfo machineMetricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/bash -c \"/usr/bin/nproc;/usr/bin/cat /proc/loadavg|/usr/bin/awk '{print $1\\\"\\n\\\"$2\\\"\\n\\\"$3}'\"");
            var cpuLines = manageProcess.Execute(machineMetricsProcessStartInfo).Split("\n");
            if (cpuLines[0].StartsWith("WARNING"))
            {
                _logger.Log(LogLevel.Warning, $"WARNING: CPU Metrics cmd results failed --> Output: {cpuLines[0]}");
            }
            else
            {
                cpu = new CPU(cpuLines);
            }
            machineMetricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/bash -c \"/usr/bin/free -m|/usr/bin/head -2|/usr/bin/tail -n +2|/usr/bin/awk '{print $2\\\"\\n\\\"$3\\\"\\n\\\"$4\\\"\\n\\\"$6\\\"\\n\\\"$7}'\"");
            var memLines = manageProcess.Execute(machineMetricsProcessStartInfo).Split("\n");
            if (memLines[0].StartsWith("WARNING"))
            {
                _logger.Log(LogLevel.Warning, $"WARNING: Memory Metrics cmd results failed --> Output: {memLines[0]}");
            }
            else
            {
                memory = new MemoryInMB(memLines);
            }
            machineMetricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/bash -c \"/usr/bin/cat /proc/uptime|/usr/bin/awk '{print int($1)}'\"");
            var uptimeLines = manageProcess.Execute(machineMetricsProcessStartInfo).Split("\n");
            if (uptimeLines[0].StartsWith("WARNING"))
            {
                _logger.Log(LogLevel.Warning, $"WARNING: Uptime Metrics cmd results failed,  --> Output: {uptimeLines[0]}");
            }
            {
                uptime = new UptimeInSeconds(uptimeLines);
            }
            machineMetrics.CPU = cpu;
            machineMetrics.MemoryInMB = memory;
            machineMetrics.UptimeInSeconds = uptime;
            return machineMetrics;
        }
    }
}
