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

        // OPTIONAL: Add string of comma separated valid exit codes, any others will be considered a WARNING or ERROR.
        public string Execute(ProcessStartInfo processStartInfo, string validCodeString = "0")
        {
            var proc = new Process { StartInfo = processStartInfo };
            int[] validCodes = validCodeString.Split(',').Select(s => int.TryParse(s, out int n) ? n : 0).ToArray();
            proc.Start();
            while (!proc.WaitForExit(10000))
            {
                proc.Kill();
                _logger.Log(LogLevel.Warning, $"WARNING: Execute CMD timed out (10s) --> Process killed.");
                return "WARNING: Execute CMD timed out (10s) --> Process killed.";
            };
            var errorOutput = proc.StandardError.ReadToEnd();
            if (errorOutput.Length > 0 || !validCodes.Contains(proc.ExitCode))
            {
                _logger.Log(LogLevel.Warning, $"WARNING: Execute CMD failed --> Exit code: {proc.ExitCode} --> {errorOutput}");
                return "WARNING: Execute CMD failed --> Exit code: " + proc.ExitCode + " --> " + errorOutput;
            }
            var standardOutput = proc.StandardOutput.ReadToEnd();
            //_logger.Log(LogLevel.Information, $"CMD OUTPUT:\n{standardOutput}");
            return standardOutput;
        }

        // Must double escape CMD arguments due to double string interpolation (C# and bash)
        public ProcessStartInfo BuildProcessStartInfo(string cmd)
        {
            //_logger.Log(LogLevel.Information, $"CMD: {cmd}");
            return new ProcessStartInfo
            {
                FileName = "/usr/bin/sudo",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"{cmd}"
            };
        }
    }
}
