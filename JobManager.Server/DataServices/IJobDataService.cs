using System.Collections.Generic;
using System.Threading.Tasks;
using JobManager.Server.Domain;

namespace JobManager.Server.DataServices
{
    public interface IJobDataService
    {
        Task<Job> Add(Job job);

        Task<IEnumerable<Job>> GetAll(JobStatus status);
    }
}