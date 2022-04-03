using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace LinuxHydraPartitionController.Api.WebHost
{
    // This class has Methods to build and execute linux processes
    public class ManageProcess
    {
        private readonly ILogger _logger;

        public ManageProcess(ILogger logger)
        {
            _logger = logger;
        }

        public string Execute(ProcessStartInfo processStartInfo)
        {
            var proc = new Process { StartInfo = processStartInfo };
            proc.Start();
            var errorOutput = proc.StandardError.ReadToEnd();
            if (errorOutput.Length > 0)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Execute CMD failed --> {errorOutput}");
            }
            var standardOutput = proc.StandardOutput.ReadToEnd();
            _logger.Log(LogLevel.Information, $"CMD OUTPUT:\n{standardOutput}");
            return standardOutput;
        }

        // Must double escape CMD arguments due to double string interpolation (C# and bash)
        public ProcessStartInfo BuildProcessStartInfo(string cmd)
        {
            _logger.Log(LogLevel.Information, $"CMD: {cmd}");
            return new ProcessStartInfo
            {
                FileName = "/usr/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"-c \"{cmd}\""
            };
        }
    }
}
