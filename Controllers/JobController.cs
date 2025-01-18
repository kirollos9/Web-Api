using AutoMapper;
using DotnetApi.Dtos;
using DotnetApi.Models;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly AddressBookContext _context;
        IMapper _mapper;
        public JobsController(AddressBookContext context)
        {
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobDto, Job>();
               
            }));
            _context = context;
        }

        [HttpGet("Jobs/{jobId}/{userId}/{searchParam}")]
        public IActionResult GetJobs(int jobId = 0, int userId = 0, string searchParam = "None")
        {
            var query = _context.Jobs.AsQueryable();

            if (jobId != 0)
            {
                query = query.Where(j => j.JobId == jobId);
            }

            if (userId != 0)
            {
                query = query.Where(j => j.UserId == userId);
            }

            if (!string.IsNullOrEmpty(searchParam) && searchParam.ToLower() != "none")
            {
                query = query.Where(j => j.Title.Contains(searchParam));
            }

            var jobs = query.ToList();
            return Ok(jobs);
        }

        [HttpGet("MyJobs")]
        public List<Job> GetMyJobs()
        {
            var userIdClaim = User.FindFirst("userId");
           

            int userId = int.Parse(userIdClaim.Value);
            var jobs = _context.Jobs.Where(j => j.UserId == userId).ToList();

            return jobs;
        }

        [HttpPut("UpsertJob")]
        public IActionResult UpsertJob(JobDto jobD)
        {
            Job job = _mapper.Map<Job>(jobD);
            var userIdClaim = User.FindFirst("userId");
            

            job.UserId = int.Parse(userIdClaim.Value);

            if (job.JobId > 0)
            {
                // Update existing job
                var existingJob = _context.Jobs.Find(job.JobId);
                if (existingJob == null)
                {
                    return NotFound("Job not found.");
                }

                existingJob.Title = job.Title;
                _context.Jobs.Update(existingJob);
            }
            else
            {
                // Create new job
                _context.Jobs.Add(job);
            }

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("Job/{jobId}")]
        public IActionResult DeleteJob(int jobId)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var job = _context.Jobs.FirstOrDefault(j => j.JobId == jobId && j.UserId == userId);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            _context.Jobs.Remove(job);
            _context.SaveChanges();

            return Ok();
        }
    }
}
