using System;
using System.Threading.Tasks;
using JobManager.Server.Domain;

namespace JobManager.Server.AppServices
{
    public interface IJobProcessor
    {
        public JobType ForJobType { get; }
        Task Process(Guid id, string payload);
    }
}