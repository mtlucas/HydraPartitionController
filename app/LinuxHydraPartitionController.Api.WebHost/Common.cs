using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LinuxHydraPartitionController.Api.WebHost.Common
{
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
}
