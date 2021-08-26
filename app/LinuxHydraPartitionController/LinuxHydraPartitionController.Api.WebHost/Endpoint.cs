using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Endpoint
    {
        public string Path { get; }
        public string Method { get; }

        internal Endpoint(string path, string method)
        {
            Path = path;
            Method = method;
        }
    }
}