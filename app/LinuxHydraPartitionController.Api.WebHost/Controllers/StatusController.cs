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
using Microsoft.AspNetCore.Http;
using LinuxHydraPartitionController.Api.WebHost.Models;

namespace LinuxHydraPartitionController.Api.WebHost.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<Status> _logger;
        private readonly IEnumerable<Status> _partitions;

        public StatusController(ILogger<Status> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            _partitions = BuildPartitions(configuration);
        }

        [HttpGet("")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Status> Get()
        {
            return _partitions;
        }

        [HttpGet("{id:int}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Status Get([FromRoute] int id)
        {
            return GetPartitionById(id);
        }

        private Status GetPartitionById(int id)
        {
            return _partitions.ToArray().Single(partition => partition.IdMatches(id));
        }

        private IEnumerable<Status> BuildPartitions(IConfiguration configuration)
        {
            String gosFilePath = configuration.GetSection("GosConfigPath").Get<String>();
            var partitions = new List<Status>();
            using (StreamReader streamReader = new StreamReader(gosFilePath))
            {
                var jsonString = streamReader.ReadToEnd();
                var gosConfig = JsonSerializer.Deserialize<GosConfig>(jsonString);
                gosConfig.machines.ForEach(machineConfig =>
                {
                    string machineName = machineConfig.machineName.ToLower();
                    if (machineName == (Environment.MachineName.ToLower().Split('.')[0]))
                    {
                        if (machineConfig.lusEnabled)
                        {
                            // LUS partition equals 0
                            var partition = new Status(_logger, 0);
                            partitions.Add(partition);
                        }
                        machineConfig.partitions.ForEach(partitionConfig =>
                        {
                            var partition = new Status(_logger, partitionConfig.partition);
                            partitions.Add(partition);
                        });
                    }
                });
            }
            return partitions;
        }
    }
}
