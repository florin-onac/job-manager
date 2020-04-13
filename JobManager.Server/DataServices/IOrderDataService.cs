using System;
using System.Threading.Tasks;
using JobManager.Server.Domain;

namespace JobManager.Server.DataServices
{
    public interface IOrderDataService
    {
        Task Add(Order order);
        Task<Order> Get(Guid id);
    }
}