using slideshow.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow
{

    public class ServiceManager
    {

        private IService[] services;

        public ServiceManager(IEnumerable<IService> services)
        {
            this.services = services.ToArray();
        }

        public async Task StartAsync()
        {
            foreach (var service in this.services)
            {
                Console.WriteLine("Starting..." + service.ToString());
                await service.StartAsync();
            }
        }

        public async Task StopAsync()
        {
            foreach (var service in this.services)
            {
                Console.WriteLine("Stopping..." + service.ToString());
                await service.StopAsync();
            }
        }
    }


}
