using Quartz;
using slideshow.core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace slideshow.scheduler.jobs
{
    public class SampleJob : IJob, IJobRegistration
    {
        public SampleJob()
        {
        }

        public Type JobType => this.GetType();

        public string GroupName => "Default";

        public string Name => this.GetType().Name;

        public string CronSchedule => "0 * * * * ?";

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => Console.WriteLine("Sample Job {0}", DateTime.Now));
        }
    }
}
