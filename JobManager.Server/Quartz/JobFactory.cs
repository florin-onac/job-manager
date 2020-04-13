using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace JobManager.Server.Quartz
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider provider;

        public JobFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;

            var scope = provider.CreateScope();

            var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;

            return new ScopedJob(scope, job);
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }

        private class ScopedJob : IJob, IDisposable
        {
            private readonly IServiceScope scope;
            private readonly IJob innerJob;

            public ScopedJob(IServiceScope scope, IJob innerJob)
            {
                this.scope = scope;
                this.innerJob = innerJob;
            }

            public Task Execute(IJobExecutionContext context) => innerJob.Execute(context);


            public void Dispose()
            {
                scope.Dispose();
                (innerJob as IDisposable)?.Dispose();
            }
        }
    }
}