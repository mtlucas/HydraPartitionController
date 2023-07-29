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
    public class Logfile
    {
        public int Id { get; }
        List<LogFileProps> logFileProps = new List<LogFileProps>();

        private readonly ILogger<Logfile> _logger;
        private readonly IConfiguration Configuration;

        internal Logfile(ILogger<Logfile> logger, int id, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            Id = id;
            var manageProcess = new ManageProcess(_logger);
        }

        public List<LogFileProps> GetLogFileProps(int Id)
        {
            String logfilePath = Configuration.GetSection("LogfilePath").Get<String>();
            var inputDirectory = new DirectoryInfo(logfilePath);
            List<FileInfo> latestFile = inputDirectory.GetFiles($"*hpu-{Id}-*.log").OrderByDescending(x => x.LastWriteTime).ToList();  // Limit to maximum of 10 latest results

            foreach (FileInfo latestFileItem in latestFile)
            {
                TimeSpan t = latestFileItem.LastWriteTimeUtc - new DateTime(1970, 1, 1);
                logFileProps.Add(new LogFileProps { logfileName = latestFileItem.Name, logfileSize = latestFileItem.Length, logfileUnixTime = (long)t.TotalSeconds });
            }
            return logFileProps;
        }
    }
}
