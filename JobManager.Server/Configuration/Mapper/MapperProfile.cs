using System.Collections.Generic;
using AutoMapper;
using JobManager.Server.Domain;
using JobManager.Server.Models;

namespace JobManager.Server.Configuration.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<JobAddModel, Job>();
            CreateMap<Job, JobModel>();

            CreateMap<IList<Job>, IList<JobModel>>();
        }
    }
}