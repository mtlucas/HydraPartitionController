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
        public LogFileProps logFileProps = new LogFileProps();

        private readonly ILogger<Logfile> _logger;
        private readonly IConfiguration Configuration;

        internal Logfile(ILogger<Logfile> logger, int id, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            Id = id;
            var manageProcess = new ManageProcess(_logger);
        }

        public LogFileProps GetLogFileProps(int Id)
        {
            String logfilePath = Configuration.GetSection("LogfilePath").Get<String>();
            var inputDirectory = new DirectoryInfo(logfilePath);
            var latestFile = inputDirectory.GetFiles($"*hpu-{Id}*.log").OrderByDescending(f => f.LastWriteTime).First();

            logFileProps.logfileName = latestFile.FullName;
            TimeSpan t = latestFile.LastWriteTimeUtc - new DateTime(1970, 1, 1);
            logFileProps.logfileUnixTime = (long)t.TotalSeconds;
            logFileProps.logfileSize = latestFile.Length;
            return logFileProps;
        }
    }
}
