using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.SchedulerConfig
{
    [DisallowConcurrentExecution]
    public abstract class AbstractJob : IJob
    {
        protected readonly ILogger<AbstractJob> _logger;
        public AbstractJob(ILogger<AbstractJob> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(context.JobInstance.ToString() + " START AT " + DateTime.Now.ToString());
            try
            {
                await JobProcess(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(context.JobInstance.ToString() + " error in JobProcess. " + ex.ToString());
            }
            _logger.LogInformation(context.JobInstance.ToString() + " END AT " + DateTime.Now.ToString());
        }

        public abstract Task JobProcess(IJobExecutionContext context);
    }
}
