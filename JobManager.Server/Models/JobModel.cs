using System;
using JobManager.Server.Domain;

namespace JobManager.Server.Models
{
    public class JobModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
