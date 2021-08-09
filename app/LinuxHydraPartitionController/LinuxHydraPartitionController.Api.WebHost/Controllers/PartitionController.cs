using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace LinuxHydraPartitionController.Api.WebHost.Controllers
{
    [ApiController]
    [Route("partitions")]
    public class PartitionController : ControllerBase
    {
        private readonly ILogger<Partition> _logger;
        private readonly IEnumerable<Partition> _partitions;

        public PartitionController(ILogger<Partition> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _partitions = GetPartitions();
        }

        [HttpGet("")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Partition> Get()
        {
            return _partitions;
        }

        [HttpPost("{id}/restart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Restart([FromRoute] int id)
        {
            GetPartitionById(id).Restart();
        }

        [HttpPost("{id}/start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Start([FromRoute] int id)
        {
            GetPartitionById(id).Start();
        }

        [HttpPost("{id}/stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Stop([FromRoute] int id)
        {
            GetPartitionById(id).Stop();
        }

        private Partition GetPartitionById(int id)
        {
            return _partitions.ToArray().Single(partition => partition.IdMatches(id));
        }

        private List<Partition> GetPartitions()
        {
            var partitions = new List<Partition>();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = "-c '/usr/bin/systemctl list-units --type=service | /usr/bin/grep gos_hpu_'"
            };
            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                var reader = process.StandardOutput;
                var output = reader.ReadToEnd();
                process.WaitForExit();
                var lines = output.Split("\n");
                var pattern = new Regex("^gos_hpu_(?<partition>\\d+)\\s+.*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (var line in lines)
                {
                    var matches = pattern.Matches(line);
                    foreach (Match match in matches)
                    {
                        var partitionString = match.Groups["partition"].Value;
                        var partitionId = int.Parse(partitionString);
                        var partition = new Partition(partitionId);
                        partitions.Add(partition);
                    }
                }
            }
            return partitions;
        }
    }
}
