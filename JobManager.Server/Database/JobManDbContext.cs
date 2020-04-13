using JobManager.Server.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobManager.Server.Database
{
    public class JobManDbContext : DbContext
    {
        public JobManDbContext(DbContextOptions<JobManDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(x => x.Amount)
                .HasColumnType("decimal(18,5)");
        }
    }
}