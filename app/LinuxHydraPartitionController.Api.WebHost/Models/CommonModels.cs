using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace LinuxHydraPartitionController.Api.WebHost.Models
{
    // GosConfig.json data types
    public class GosConfig
    {
        public List<MachineConfig> machines { get; set; }
    }
    public class MachineConfig
    {
        public string machineName { get; set; }
        public List<PartitionConfig> partitions { get; set; }
    }
    public class PartitionConfig
    {
        public int partition { get; set; }
    }

    // Endpoint data types
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

    // Machine Metrics data types
    public class MachineMetrics
    {
        public CPU CPU { get; set; }
        public MemoryInMB MemoryInMB { get; set; }
        public UptimeInSeconds UptimeInSeconds { get; set; }
    }
    public class CPU
    {
        public int Cores { get; set; } = 0;
        public float Load1min { get; set; } = 0;
        public float Load5min { get; set; } = 0;
        public float Load15min { get; set; } = 0;

        internal CPU(string[] cpuLines)
        {
            Cores = Convert.ToInt32(cpuLines[0].ToString());
            Load1min = Convert.ToSingle(cpuLines[1].ToString());
            Load5min = Convert.ToSingle(cpuLines[2].ToString());
            Load15min = Convert.ToSingle(cpuLines[3].ToString());
        }
    }
    public class MemoryInMB
    {
        public int Total { get; set; } = 0;
        public int Used { get; set; } = 0;
        public int Free { get; set; } = 0;
        public int Buffers { get; set; } = 0;
        public int Available { get; set; } = 0;

        internal MemoryInMB(string[] memLines)
        {
            Total = Convert.ToInt32(memLines[0].ToString());
            Used = Convert.ToInt32(memLines[1].ToString());
            Free = Convert.ToInt32(memLines[2].ToString());
            Buffers = Convert.ToInt32(memLines[3].ToString());
            Available = Convert.ToInt32(memLines[4].ToString());
        }
    }
    public class UptimeInSeconds
    {
        public int Uptime { get; set; } = 0;

        internal UptimeInSeconds(string[] uptimeLines)
        {
            Uptime = Convert.ToInt32(uptimeLines[0].ToString());
        }
    }
    public class LogFileProps
    {
        public string logfileName { get; set; }
        public long logfileUnixTime { get; set; }
        public long logfileSize { get; set; }
    }
}
