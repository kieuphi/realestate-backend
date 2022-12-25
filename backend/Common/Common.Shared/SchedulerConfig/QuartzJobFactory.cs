using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.SchedulerConfig
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle triggerFiredBundle, IScheduler scheduler)
        {
            var jobDetail = triggerFiredBundle.JobDetail;
            return (IJob)_serviceProvider.GetService(jobDetail.JobType);
        }

        public void ReturnJob(IJob job) { }
    }
}
