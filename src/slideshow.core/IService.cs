using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace slideshow.core
{
    public interface IService
    {
        Task StartAsync();
        Task StopAsync();
    }
}
