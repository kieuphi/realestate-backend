using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Common.Shared.SchedulerConfig
{
    public class QuartzHostedService : IHostedService, IDisposable
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobFactory jobFactory;
        private readonly IEnumerable<GenericSchedulerSetting> _listJobMetadata;
        private readonly ILogger<QuartzHostedService> _logger;
        public IScheduler Scheduler { get; set; }

        public QuartzHostedService(ISchedulerFactory schedulerFactory, IEnumerable<GenericSchedulerSetting> listJobMetadata,
            IJobFactory jobFactory, ILogger<QuartzHostedService> logger)
        {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
            this._listJobMetadata = listJobMetadata;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler start success.");
            Scheduler = await schedulerFactory.GetScheduler();
            Scheduler.JobFactory = jobFactory;
            foreach (GenericSchedulerSetting jobMetadata in _listJobMetadata)
            {
                var job = CreateJob(jobMetadata);
                var trigger = CreateTrigger(jobMetadata);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        private ITrigger CreateTrigger(GenericSchedulerSetting jobMetadata)
        {
            return TriggerBuilder.Create()
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription($"{jobMetadata.JobName}")
                .Build();
        }

        private IJobDetail CreateJob(GenericSchedulerSetting jobMetadata)
        {
            return JobBuilder
                .Create(jobMetadata.JobType)
                .WithIdentity(jobMetadata.JobName)
                .WithDescription($"{jobMetadata.JobName}")
                .Build();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Scheduler dispose.");
        }
    }
}
