using System.Diagnostics;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Partition
    {
        public int Id { get; }
        public string RestartPath { get; }
        
        private readonly ProcessStartInfo _processStartInfo;
        
        internal Partition(int id)
        {
            Id = id;
            RestartPath = $"/restart/{id}";
            _processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", 
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"-c '/usr/bin/systemctl restart gos_{id}'"
            }; 
        }

        public bool IdMatches(int id)
        {
            return Id.Equals(id);
        }

        public void Restart()
        {
            var proc = new Process { StartInfo = _processStartInfo };
            proc.Start();
            var error = proc.StandardError;
        }
    }
}
