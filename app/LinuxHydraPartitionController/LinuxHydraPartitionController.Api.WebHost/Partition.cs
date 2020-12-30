using System;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Partition
    {
        private readonly int _id;
        internal Partition(int id)
        {
            _id = id;
        }

        public bool IdMatches(int id)
        {
            return _id.Equals(id);
        }

        public void Restart()
        {
            
        }
    }
}
