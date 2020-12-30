using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace LinuxHydraPartitionController.Api.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PartitionController : ControllerBase
    {
        private readonly ILogger<Partition> _logger;
        private readonly IEnumerable<Partition> _partitions;
        
        public PartitionController(ILogger<Partition> logger)
        {
            _logger = logger;
            //Get partitions
            _partitions = Enumerable.Range(1, 5).Select(index => new Partition(1)).ToArray();
        }

        [HttpGet]
        [Produces("application/json")]
        public IEnumerable<Partition> Get()
        {
            return _partitions;
        }

        [HttpPost]
        [Route("[controller]")]
        public void Restart(int id)
        {
            _partitions.ToArray()
                .First(partition => partition.IdMatches(id))
                .Restart();
        }
    }
}
