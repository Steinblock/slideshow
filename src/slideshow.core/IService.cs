using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace slideshow.core
{
    public interface IService
    {
        string Name { get; }
        ServiceStatus Status { get; }
        Task StartAsync();
        Task StopAsync();
    }

    public enum ServiceStatus
    {
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        Faulted = 5
    }
}
