using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using LinuxHydraPartitionController.Api.WebHost.Models;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Status
    {
        public int Id { get; }
        public Endpoint StatusEndpoint { get; }
        public string ServiceStatus => GetStatus();

        public static readonly IEnumerable<Status> _partitions;
        private readonly ILogger<Status> _logger;
        private readonly ProcessStartInfo _statusProcessStartInfo;

        internal Status(ILogger<Status> logger, int id)
        {
            _logger = logger;
            Id = id;

            StatusEndpoint = new Endpoint($"/partitions/{Id}", "GET");
            _statusProcessStartInfo = BuildProcessStartInfo("status");
        }

        public bool IdMatches(int id)
        {
            return Id.Equals(id);
        }

        private string GetStatus()
        {
            var statusString = Execute(_statusProcessStartInfo);
            var statusLines = statusString.Split("\n");
            var activeLine = statusLines[2].Trim();
            var isRunning = activeLine.StartsWith("Active: active (running)");
            return isRunning ? "Running" : "Stopped";
        }

        private string Execute(ProcessStartInfo processStartInfo)
        {
            var proc = new Process { StartInfo = processStartInfo };
            proc.Start();
            var errorOutput = proc.StandardError.ReadToEnd();
            if (errorOutput.Length > 0)
            {
                _logger.Log(LogLevel.Error, $"ERROR: For partition {Id} --> {errorOutput}");
            }
            return proc.StandardOutput.ReadToEnd();
        }

        private ProcessStartInfo BuildProcessStartInfo(string state)
        {
            return new ProcessStartInfo
            {
                FileName = "/usr/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"-c \"/usr/bin/sudo /usr/bin/systemctl {state} gos_hpu_{Id}.service\""
            };
        }
    }
}
