using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.SchedulerConfig
{
    public class GenericSchedulerSetting
    {
        public Guid JobId { get; set; }
        public Type JobType { get; }
        public string JobName { get; }
        public string CronExpression { get; }

        public GenericSchedulerSetting(Guid Id, Type jobType, string jobName, string cronExpression)
        {
            JobId = Id;
            JobType = jobType;
            JobName = jobName;
            CronExpression = cronExpression;
        }
    }
}
