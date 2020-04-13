using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobManager.Server.Database;
using JobManager.Server.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobManager.Server.DataServices
{
    public class JobDataService : IJobDataService
    {
        private readonly JobManDbContext context;

        public JobDataService(JobManDbContext context)
        {
            this.context = context;
        }

        public async Task<Job> Add(Job job)
        {
            job.Id = Guid.NewGuid();
            job.Status = JobStatus.Ready;
            job.Created = DateTime.UtcNow;
            job.Updated = null;

            var added = await context.Jobs.AddAsync(job);

            await context.SaveChangesAsync();

            return added.Entity;
        }

        public async Task<IEnumerable<Job>> GetAll(JobStatus status)
        {
            var entities = await context.Jobs.AsNoTracking()
                .Where(x => x.Status == status)
                .ToListAsync();

            return entities;
        }

        public async Task UpdateStatus(Guid id, JobStatus status)
        {
            var entity = await context.Jobs.FirstAsync(x => x.Id == id);

            entity.Status = status;
            entity.Updated = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }
    }
}