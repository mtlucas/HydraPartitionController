using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Partition
    {
        public int Id { get; }
        public string StopPath { get; }
        public string StartPath { get; }
        public string RestartPath { get; }

        private readonly ILogger<Partition> _logger;
        private readonly ProcessStartInfo _restartProcessStartInfo;
        private readonly ProcessStartInfo _startProcessStartInfo;
        private readonly ProcessStartInfo _stopProcessStartInfo;

        internal Partition(ILogger<Partition> logger, int id)
        {
            _logger = logger;
            Id = id;

            StartPath = $"/partitions/{Id}/start";
            _startProcessStartInfo = BuildProcessStartInfo("start");

            StopPath = $"/partitions/{Id}/stop";
            _stopProcessStartInfo = BuildProcessStartInfo("stop");

            RestartPath = $"/partitions/{Id}/restart";
            _restartProcessStartInfo = BuildProcessStartInfo("restart");
        }

        public bool IdMatches(int id)
        {
            return Id.Equals(id);
        }

        public void Restart()
        {
            _logger.Log(LogLevel.Critical, $"Restarting partition {Id}.");
            Execute(_restartProcessStartInfo);
            _logger.Log(LogLevel.Critical, $"Finished restarting partition {Id}.");
        }

        public void Start()
        {
            _logger.Log(LogLevel.Critical, $"Starting partition {Id}.");
            Execute(_startProcessStartInfo);
            _logger.Log(LogLevel.Critical, $"Finished starting partition {Id}.");
        }

        public void Stop()
        {
            _logger.Log(LogLevel.Critical, $"Stopping partition {Id}.");
            Execute(_stopProcessStartInfo);
            _logger.Log(LogLevel.Critical, $"Finished stopping partition {Id}.");
        }

        private void Execute(ProcessStartInfo processStartInfo)
        {
            var proc = new Process {StartInfo = processStartInfo};
            proc.Start();
            var error = proc.StandardError;
            var output = error.ReadToEnd();
            _logger.Log(LogLevel.Error, $"For partition {Id}: {output}");
        }

        private ProcessStartInfo BuildProcessStartInfo(string state)
        {
            var arguments = $"-c \"/usr/bin/systemctl {state} gos_hpu_{Id}.service\"";
            _logger.Log(LogLevel.Critical, arguments);
            return new ProcessStartInfo
            {
                FileName = "/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments
            };
        }
    }
}
