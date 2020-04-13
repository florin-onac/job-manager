using System;
using System.Threading.Tasks;
using JobManager.Server.Database;
using JobManager.Server.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobManager.Server.DataServices
{
    public class OrderDataService : IOrderDataService
    {
        private readonly JobManDbContext context;

        public OrderDataService(JobManDbContext context)
        {
            this.context = context;
        }

        public async Task Add(Order order)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }

        public async Task<Order> Get(Guid id)
        {
            return await context.Orders.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}