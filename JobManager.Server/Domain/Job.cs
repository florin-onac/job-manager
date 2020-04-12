using System;

namespace JobManager.Server.Domain
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public string Payload { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
