using System;
using System.IO;
using System.Threading.Tasks;
using JobManager.Server.DataServices;
using JobManager.Server.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobManager.Server.AppServices
{
    public class OrderExporter : IJobProcessor
    {
        private readonly IJobDataService jobDataService;
        private readonly IOrderDataService orderDataService;
        private readonly ILogger<OrderExporter> logger;
        private readonly IConfiguration configuration;

        public OrderExporter(
            IJobDataService jobDataService,
            IOrderDataService orderDataService,
            ILogger<OrderExporter> logger,
            IConfiguration configuration)
        {
            this.jobDataService = jobDataService;
            this.orderDataService = orderDataService;
            this.logger = logger;
            this.configuration = configuration;

            Directory.CreateDirectory(configuration["ExportPath"]);
        }

        public JobType ForJobType => JobType.OrderExporter;

        public async Task Process(Guid id, string payload)
        {
            await jobDataService.UpdateStatus(id, JobStatus.Started);

            var status = JobStatus.Completed;

            try
            {
                var orderId = Guid.Parse(payload);

                var order = await orderDataService.Get(orderId);

                var fileName = Path.Combine(configuration["ExportPath"], $"{orderId}.txt");

                File.WriteAllText(fileName, JsonConvert.SerializeObject(order));
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