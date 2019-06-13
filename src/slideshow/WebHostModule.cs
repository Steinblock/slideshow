using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Modules;
using slideshow.core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow
{
    public class WebHostModule : NinjectModule
    {
        private string[] args;

        public WebHostModule(string[] args)
        {
            this.args = args;
        }

        public override void Load()
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices(configure => configure.AddSingleton(this.Kernel))
                .UseStartup<Startup>()
                // use fixed ports for now  to avoid docker changing the ports 
                // back to 80 / 443
                .UseUrls("http://+:5000", "https://+:5001"); 

            var host = builder.Build();

            this.Bind<IService>().To<WebHostService>()
                .WithConstructorArgument("host", host);

        }

        private class WebHostService : IService
        {
            private IWebHost host;
            private CancellationTokenSource cts;

            public WebHostService(IWebHost host)
            {
                this.host = host;
                this.cts = new CancellationTokenSource();
            }

            public Task StartAsync()
            {
                host.RunAsync(cts.Token);
                return Task.CompletedTask;
            }

            public async Task StopAsync()
            {
                await host.StopAsync();
            }

        }

    }

}
