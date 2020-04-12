using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using JobManager.Server.DataServices;
using JobManager.Server.Domain;
using JobManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IJobDataService dataService;

        public JobsController(
            IMapper mapper,
            IJobDataService dataService)
        {
            this.mapper = mapper;
            this.dataService = dataService;
        }

        //[Route("")]
        [HttpPost]
        public async Task<IActionResult> AddJob(JobAddModel job)
        {
            var entity = mapper.Map<JobAddModel, Job>(job);

            var added = await dataService.Add(entity);

            var result = mapper.Map<Job, JobModel>(added);

            return StatusCode((int) HttpStatusCode.Created, result);
        }

        //[Route("")]
        [HttpGet]
        public async Task<IActionResult> GetJobs(JobStatus status)
        {
            var data = await dataService.GetAll(status);

            var result = mapper.Map<IEnumerable<Job>, IEnumerable<JobModel>>(data);

            return Ok(result);
        }
    }
}