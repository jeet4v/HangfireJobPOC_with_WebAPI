using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace HangfireJobDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTestController : ControllerBase
    {
        private readonly IJobTestService _jobTestService;
        private readonly IBackgroundJobClient _backgroundJobClient; // Interface provided by Hangfire for Enqueue & Schedule jobs
        private readonly IRecurringJobManager _recurringJobManager; // Interface provided by Hangfire for Recurring jobs

        public JobTestController(IJobTestService jobTestService, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _jobTestService = jobTestService;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        [HttpGet("/FireAndForgetJob")]
        public ActionResult CreateFireAndForgetJob()
        {
            _backgroundJobClient.Enqueue(() => _jobTestService.FireAndForgetJob());
            return Ok("Message from CreateFireAndForgetJob");
        }

        [HttpGet("/DelayedJob")]
        public ActionResult DelayedJob()
        {
            _backgroundJobClient.Schedule(() => _jobTestService.DelayedJob(), TimeSpan.FromSeconds(60));
            return Ok("Message from DelayedJob");
        }

        [HttpGet("/ContinuationJob")]
        public ActionResult ContinuationJob()
        {
            var parentJobID = _backgroundJobClient.Enqueue(() => _jobTestService.FireAndForgetJob());
            _backgroundJobClient.ContinueJobWith(parentJobID, () => _jobTestService.ContinuationJob());
            return Ok("Message from ContinuationJob");
        }

        [HttpGet("/ReccuringJob")]
        public ActionResult ReccuringJob()
        {
            _recurringJobManager.AddOrUpdate("RecurringJobID", () => _jobTestService.ReccuringJob(), Cron.Minutely);
            return Ok("Message from ReccuringJob");
        }

    }
}
