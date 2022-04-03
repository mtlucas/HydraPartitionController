﻿using System;
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
        public MachineMetrics machineMetrics = new MachineMetrics();

        private readonly ILogger<Metrics> _logger;

        public Metrics(ILogger<Metrics> logger)
        {
            _logger = logger;
        }
        public MachineMetrics GetMetrics()
        {
            var manageProcess = new ManageProcess(_logger);
            ProcessStartInfo machineMetricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/nproc;/usr/bin/cat /proc/loadavg|/usr/bin/awk '{print $1\\\"\\n\\\"$2\\\"\\n\\\"$3}'");
            var cpuLines = manageProcess.Execute(machineMetricsProcessStartInfo).Split("\n");
            _logger.LogInformation($"CPU Line 0 and 3: {cpuLines[0]} {cpuLines[3]}");
            int temp = Convert.ToInt32(cpuLines[0].ToString());
            _logger.LogInformation($"CPU Line 0 Int: {temp}");
            machineMetrics.CPU.Cores.Equals(temp);
            machineMetrics.CPU.Load1min = Convert.ToSingle(cpuLines[1].ToString());
            machineMetrics.CPU.Load5min = Convert.ToSingle(cpuLines[2].ToString());
            machineMetrics.CPU.Load15min = Convert.ToSingle(cpuLines[3].ToString());
            machineMetricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/free -m|/usr/bin/head -2|/usr/bin/tail -n +2|/usr/bin/awk '{print $2\\\"\\n\\\"$3\\\"\\n\\\"$4\\\"\\n\\\"$6\\\"\\n\\\"$7}'");
            var memlines = manageProcess.Execute(machineMetricsProcessStartInfo).Split("\n");
            machineMetrics.MemoryInMB.Total = Convert.ToInt32(memlines[0].ToString());
            machineMetrics.MemoryInMB.Used = Convert.ToInt32(memlines[1].ToString());
            machineMetrics.MemoryInMB.Free = Convert.ToInt32(memlines[2].ToString());
            machineMetrics.MemoryInMB.Buffers = Convert.ToInt32(memlines[3].ToString());
            machineMetrics.MemoryInMB.Available = Convert.ToInt32(memlines[4].ToString());
            machineMetricsProcessStartInfo = manageProcess.BuildProcessStartInfo("/usr/bin/cat /proc/uptime|/usr/bin/awk '{print int($1)}'");
            var uptimeLines = manageProcess.Execute(machineMetricsProcessStartInfo).Split("\n");
            machineMetrics.UptimeInSeconds.Uptime = Convert.ToInt32(uptimeLines[0].ToString());
            return machineMetrics;
        }
    }
}
