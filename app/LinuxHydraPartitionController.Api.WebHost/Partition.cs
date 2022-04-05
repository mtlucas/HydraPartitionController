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
    public class Partition
    {
        public int Id { get; }
        public Endpoint StopEndpoint { get; }
        public Endpoint StartEndpoint { get; }
        public Endpoint RestartEndpoint { get; }
        public Endpoint StatusEndpoint { get; }
        public string ServiceStatus => GetStatus();

        public static readonly IEnumerable<Partition> _partitions;
        private readonly ILogger<Partition> _logger;
        private readonly ProcessStartInfo _restartProcessStartInfo;
        private readonly ProcessStartInfo _startProcessStartInfo;
        private readonly ProcessStartInfo _stopProcessStartInfo;
        private readonly ProcessStartInfo _statusProcessStartInfo;

        internal Partition(ILogger<Partition> logger, int id)
        {
            _logger = logger;
            Id = id;

            var manageProcess = new ManageProcess(_logger);
            const string systemctl = "/usr/bin/sudo /usr/bin/systemctl ";

            StartEndpoint = new Endpoint($"/partitions/{Id}/start", "POST");
            _startProcessStartInfo = manageProcess.BuildProcessStartInfo(systemctl + $"start gos_hpu_{Id}.service");

            StopEndpoint = new Endpoint($"/partitions/{Id}/stop", "POST");
            _stopProcessStartInfo = manageProcess.BuildProcessStartInfo(systemctl + $"stop gos_hpu_{Id}.service");

            RestartEndpoint = new Endpoint($"/partitions/{Id}/restart", "POST");
            _restartProcessStartInfo = manageProcess.BuildProcessStartInfo(systemctl + $"restart gos_hpu_{Id}.service");

            StatusEndpoint = new Endpoint($"/partitions/{Id}", "GET");
            _statusProcessStartInfo = manageProcess.BuildProcessStartInfo(systemctl + $"status gos_hpu_{Id}.service");
        }

        public bool IdMatches(int id)
        {
            return Id.Equals(id);
        }

        public void Restart()
        {
            var manageProcess = new ManageProcess(_logger);
            _logger.Log(LogLevel.Critical, $"Restarting partition {Id}.");
            manageProcess.Execute(_restartProcessStartInfo);
            _logger.Log(LogLevel.Critical, $"Finished restarting partition {Id}.");
        }

        public void Start()
        {
            var manageProcess = new ManageProcess(_logger);
            _logger.Log(LogLevel.Critical, $"Starting partition {Id}.");
            manageProcess.Execute(_startProcessStartInfo);
            _logger.Log(LogLevel.Critical, $"Finished starting partition {Id}.");
        }

        public void Stop()
        {
            var manageProcess = new ManageProcess(_logger);
            _logger.Log(LogLevel.Critical, $"Stopping partition {Id}.");
            manageProcess.Execute(_stopProcessStartInfo);
            _logger.Log(LogLevel.Critical, $"Finished stopping partition {Id}.");
        }

        private string GetStatus()
        {
            var manageProcess = new ManageProcess(_logger);
            var statusString = manageProcess.Execute(_statusProcessStartInfo);
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
