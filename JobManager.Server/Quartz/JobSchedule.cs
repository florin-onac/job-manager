using System;

namespace JobManager.Server.Quartz
{
    public class JobSchedule
    {
        public Type JobType { get; }

        public string CronExpression { get; }

        public JobSchedule(Type type, string cronExpression)
        {
            JobType = type;
            CronExpression = cronExpression;
        }
    }
}