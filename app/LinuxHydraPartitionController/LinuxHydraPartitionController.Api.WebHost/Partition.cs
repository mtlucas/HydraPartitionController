using System.Diagnostics;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Partition
    {
        private readonly int _id;
        private readonly ProcessStartInfo _processStartInfo;
        
        internal Partition(int id)
        {
            _id = id;
            _processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", 
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"-c 'sudo /usr/bin/systemctl restart gos_{id}'"
            }; 
        }

        public bool IdMatches(int id)
        {
            return _id.Equals(id);
        }

        public void Restart()
        {
            var proc = new Process { StartInfo = _processStartInfo };
            proc.Start();
            var error = proc.StandardError;
        }
    }
}
