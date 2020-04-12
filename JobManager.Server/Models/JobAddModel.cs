using JobManager.Server.Domain;

namespace JobManager.Server.Models
{
    public class JobAddModel
    {
        public string Name { get; set; }
        public JobType Type { get; set; }
        public string Payload { get; set; }
    }
}
