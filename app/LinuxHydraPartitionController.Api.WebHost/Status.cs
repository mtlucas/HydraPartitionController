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

            var manageProcess = new ManageProcess(_logger);

            StatusEndpoint = new Endpoint($"/partitions/{Id}", "GET");
            _statusProcessStartInfo = manageProcess.BuildProcessStartInfo($"/usr/bin/sudo /usr/bin/systemctl status gos_hpu_{Id}.service");
        }

        public bool IdMatches(int id)
        {
            return Id.Equals(id);
        }

        private string GetStatus()
        {
            var manageProcess = new ManageProcess(_logger);
            var statusString = manageProcess.Execute(_statusProcessStartInfo, "0,3");
            var statusLines = statusString.Split("\n");
            if (statusLines[0].StartsWith("WARNING"))
            {
                _logger.Log(LogLevel.Warning, $"{statusLines[0]}");
                return "Error";
            }
            else
            {
                var activeLine = statusLines[2].Trim();
                var isRunning = activeLine.StartsWith("Active: active (running)");
                return isRunning ? "Running" : "Stopped";
            }
        }
    }
}
