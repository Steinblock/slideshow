using Ninject;
using Quartz;
using Quartz.Impl;
using slideshow.core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace slideshow.scheduler
{
    public class SchedulerService : IService
    {
        private IScheduler scheduler;
        private IKernel kernel;
        private IEnumerable<IJobRegistration> jobRegistrations;

        public SchedulerService(IKernel kernel, IEnumerable<IJobRegistration> jobRegistrations)
        {
            this.kernel = kernel;
            this.jobRegistrations = jobRegistrations;
        }

        public async Task StartAsync()
        {
            // Grab the Scheduler instance from the Factory
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);
            var scheduler = await factory.GetScheduler();
            scheduler.JobFactory = new NinjectJobFactory(kernel);

            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            foreach (var jobRegistration in this.jobRegistrations)
            {
                IJobDetail jobDetail = JobBuilder.Create(jobRegistration.JobType)
                    .WithIdentity(jobRegistration.Name, jobRegistration.GroupName)
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithCronSchedule(jobRegistration.CronSchedule)
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(jobDetail, trigger);

            }

            this.scheduler = scheduler;
        }

        public async Task StopAsync()
        {
            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();
        }
    }
}
