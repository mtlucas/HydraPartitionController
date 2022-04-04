using System;
using System.Collections.Generic;


namespace LinuxHydraPartitionController.Api.WebHost.Models
{
    // GosConfig.json data models
    public class GosConfig
    {
        public List<MachineConfig> machines { get; set; }
    }
    public class MachineConfig
    {
        public List<PartitionConfig> partitions { get; set; }
    }
    public class PartitionConfig
    {
        public int partition { get; set; }
    }

    // Endpoint data model
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

    // Machine Metrics data models
    public class MachineMetrics
    {
        public CPU CPU { get; set; } = null!;
        public MemoryInMB MemoryInMB { get; set; } = null!;
        public UptimeInSeconds UptimeInSeconds { get; set; } = null!;
    }
    public class CPU
    {
        public int Cores { get; set; } = 0;
        public float Load1min { get; set; } = 0;
        public float Load5min { get; set; } = 0;
        public float Load15min { get; set; } = 0;
    }
    public class MemoryInMB
    {
        public int Total { get; set; } = 0;
        public int Used { get; set; } = 0;
        public int Free { get; set; } = 0;
        public int Buffers { get; set; } = 0;
        public int Available { get; set; } = 0;
    }
    public class UptimeInSeconds
    {
        public int Uptime { get; set; } = 0;
    }
}
