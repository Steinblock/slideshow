using System;
using System.Collections.Generic;
using System.Text;

namespace slideshow.core
{
    public interface IJobRegistration
    {
        Type JobType { get; }
        string GroupName { get; }
        string Name { get; }
        string CronSchedule { get; }
    }
}
