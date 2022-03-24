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
using LinuxHydraPartitionController.Api.WebHost.Common;

namespace LinuxHydraPartitionController.Api.WebHost.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<Partition> _logger;
        private readonly IEnumerable<Partition> _partitions;

        public StatusController(ILogger<Partition> logger, IConfiguration configuration)
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
        public IEnumerable<Partition> Get()
        {
            return _partitions;
        }

        [HttpGet("{id:int}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Partition Get([FromRoute] int id)
        {
            return GetPartitionById(id);
        }
        private Partition GetPartitionById(int id)
        {
            return _partitions.ToArray().Single(partition => partition.IdMatches(id));
        }

        private IEnumerable<Partition> BuildPartitions(IConfiguration configuration)
        {
            String gosFilePath = configuration.GetSection("GosConfigPath").Get<String>();
            var partitions = new List<Partition>();
            using (StreamReader streamReader = new StreamReader(gosFilePath))
            {
                var jsonString = streamReader.ReadToEnd();
                var gosConfig = JsonSerializer.Deserialize<GosConfig>(jsonString);
                gosConfig.machines.ForEach(machineConfig =>
                {
                    machineConfig.partitions.ForEach(partitionConfig =>
                    {
                        var partition = new Partition(_logger, partitionConfig.partition);
                        partitions.Add(partition);
                    });
                });
            }
            return partitions;
        }
    }
}
