using System.Diagnostics;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Partition
    {
        public int Id { get; }
        public string StopPath { get; }
        public string StartPath { get; }
        public string RestartPath { get; }

        private readonly ProcessStartInfo _restartProcessStartInfo;
        private readonly ProcessStartInfo _startProcessStartInfo;
        private readonly ProcessStartInfo _stopProcessStartInfo;

        internal Partition(int id)
        {
            Id = id;
            
            StartPath = $"{Id}/start";
            _startProcessStartInfo = BuildProcessStartInfo("start");
            
            StopPath = $"{Id}/stop";
            _stopProcessStartInfo = BuildProcessStartInfo("stop");
            
            RestartPath = $"{Id}/restart";
            _restartProcessStartInfo = BuildProcessStartInfo("restart");
        }

        public bool IdMatches(int id)
        {
            return Id.Equals(id);
        }

        public void Restart()
        {
            Execute(_restartProcessStartInfo);  
        }

        public void Start()
        {
            Execute(_startProcessStartInfo);  
        }

        public void Stop()
        {
            Execute(_stopProcessStartInfo);        }

        private void Execute(ProcessStartInfo processStartInfo)
        {
            var proc = new Process {StartInfo = processStartInfo};
            proc.Start();
            var error = proc.StandardError;
        }

        private ProcessStartInfo BuildProcessStartInfo(string state)
        {
            return new ProcessStartInfo
            {
                FileName = "/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"-c '/usr/bin/systemctl {state} gos_{Id}'"
            };
        }
}
}
