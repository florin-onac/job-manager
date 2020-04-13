using System;
using System.Threading.Tasks;
using JobManager.Server.DataServices;
using JobManager.Server.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobManager.Server.AppServices
{
    public class OrderImporter : IJobProcessor
    {
        private readonly IJobDataService jobDataService;
        private readonly IOrderDataService orderDataService;
        private readonly ILogger<OrderImporter> logger;

        public OrderImporter(
            IJobDataService jobDataService,
            IOrderDataService orderDataService,
            ILogger<OrderImporter> logger)
        {
            this.jobDataService = jobDataService;
            this.orderDataService = orderDataService;
            this.logger = logger;
        }

        public JobType ForJobType => JobType.OrderImporter;

        public async Task Process(Guid id, string payload)
        {
            await jobDataService.UpdateStatus(id, JobStatus.Started);

            var status = JobStatus.Completed;

            try
            {
                var order = JsonConvert.DeserializeObject<Order>(payload);

                await orderDataService.Add(order);
            }
            catch (Exception e)
            {
                status = JobStatus.Failed;
                logger.LogError(e, "Processing of job {0} failed.", id);
            }

            await jobDataService.UpdateStatus(id, status);
        }
    }
}