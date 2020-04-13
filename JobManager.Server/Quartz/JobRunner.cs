using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobManager.Server.AppServices;
using JobManager.Server.Database;
using JobManager.Server.DataServices;
using JobManager.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace JobManager.Server.Quartz
{
    [DisallowConcurrentExecution]
    public class JobRunner : IJob
    {
        private readonly IEnumerable<IJobProcessor> processors;
        private readonly IJobDataService dataService;

        public JobRunner(
            IEnumerable<IJobProcessor> processors,
            IJobDataService dataService)
        {
            this.processors = processors;
            this.dataService = dataService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobs = await dataService.GetAll(JobStatus.Ready);

            foreach (var job in jobs.OrderBy(x => x.Type))
            {
                var processor = processors.FirstOrDefault(x => x.ForJobType == job.Type);
                if (processor != null)
                {
                    await processor.Process(job.Id, job.Payload);
                }
            }
        }
    }
}