﻿using System;
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
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var partitionIdsSection = configuration.GetSection("PartitionIds") ?? throw new ArgumentNullException(nameof(configuration));
            var partitionIds = partitionIdsSection.Get<List<int>>() ?? throw new ArgumentNullException(nameof(configuration));
            _partitions = partitionIds
                .Select(index => new Partition(_logger, index))
                .ToArray();
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

        [HttpPost("restart/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Restart([FromRoute] int id)
        {
            GetPartitionById(id).Restart();
        }

        [HttpPost("{id:int}/start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void Start([FromRoute] int id)
        {
            GetPartitionById(id).Start();
        }

        [HttpPost("{id:int}/stop")]
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
    }
}
