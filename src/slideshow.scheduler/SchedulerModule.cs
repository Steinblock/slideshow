using Ninject.Modules;
using slideshow.core;
using slideshow.scheduler.jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace slideshow.scheduler
{
    public class SchedulerModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IService>().To<SchedulerService>();
            this.Bind<IJobRegistration>().To<SampleJob>();
        }
    }
}
