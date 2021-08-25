using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Partition
    {
        public int Id { get; }
        public Endpoint StopEndpoint { get; }
        public Endpoint StartEndpoint { get; }
        public Endpoint RestartEndpoint { get; }
        public Endpoint StatusEndpoint { get; }
        public string Status => GetStatus();

        private readonly ILogger<Partition> _logger;
        private readonly ProcessStartInfo _restartProcessStartInfo;
        private readonly ProcessStartInfo _startProcessStartInfo;
        private readonly ProcessStartInfo _stopProcessStartInfo;
        private readonly ProcessStartInfo _statusProcessStartInfo;

        internal Partition(ILogger<Partition> logger, int id)
        {
            _logger = logger;
            Id = id;

            StartEndpoint = new Endpoint($"/partitions/{Id}/start", "POST");
            _startProcessStartInfo = BuildProcessStartInfo("start");

            StopEndpoint = new Endpoint($"/partitions/{Id}/stop", "POST");
            _stopProcessStartInfo = BuildProcessStartInfo("stop");

            RestartEndpoint = new Endpoint($"/partitions/{Id}/restart", "POST");
            _restartProcessStartInfo = BuildProcessStartInfo("restart");

            StatusEndpoint = new Endpoint($"/partitions/{Id}", "GET");
            _statusProcessStartInfo = BuildProcessStartInfo("status");
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

        private string GetStatus()
        {
            var statusString = Execute(_statusProcessStartInfo);
            var statusLines = statusString.Split("\\n");
            if (statusLines.Length < 3)
            {
                return statusString;
            }


            var activeLine = statusLines[2].Trim();
            if (activeLine.StartsWith("Active: active (running)"))
            {
                return "Running";
            }

            var loadedLine = statusLines[1].Trim();
            if (loadedLine.Equals($"Loaded: loaded (/etc/systemd/system/gos_hpu_{Id}.service; enabled; vendor preset: disabled)"))
            {
                return "Stopped";
            }

            if (loadedLine.Equals($"Loaded: loaded (/etc/systemd/system/gos_hpu_{Id}.service; disabled; vendor preset: disabled)"))
            {
                return "Disabled";
            }

            return statusString;
        }


        private string Execute(ProcessStartInfo processStartInfo)
        {
            var proc = new Process { StartInfo = processStartInfo };
            proc.Start();
            var errorOutput = proc.StandardError.ReadToEnd();
            if (errorOutput.Length > 0)
            {
                _logger.Log(LogLevel.Error, $"For partition {Id}: {errorOutput}");
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
