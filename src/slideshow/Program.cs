using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ninject;
using System;
using System.Linq;
using Topshelf;
using Topshelf.Runtime.DotNetCore;
using TopShelf.Ninject;

// cd /d d:\vortrag\slideshow\src\slideshow
// dotnet publish -c Debug -r win10-x64
// cd /d D:\vortrag\slideshow\src\slideshow\bin\Debug\netcoreapp2.1\win10-x64\publish
// 
namespace slideshow
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var kernel = new StandardKernel(new WebHostModule(args));
            //kernel.Load("slideshow.*.dll");
            kernel.Load("slideshow.data.dll");
            kernel.Load("slideshow.db.dll");

            if ((Environment.GetEnvironmentVariable("DATABASE_URL") ?? String.Empty).StartsWith("postgres"))
            {
                kernel.Load("slideshow.db.postgres.dll");
            }
            else
            {
                kernel.Load("slideshow.db.sqlite.dll");
            }

            kernel.Load("slideshow.scheduler.dll");
            kernel.Load("slideshow.web.dll");



            var host = HostFactory.New(x =>
            {

                x.UseEnvironmentBuilder(c => new DotNetCoreEnvironmentBuilder(c));

                x.UseNinject(kernel.GetModules().ToArray());

                x.Service<ServiceManager>(s =>
                {
                    s.ConstructUsing(serviceFactory => kernel.Get<ServiceManager>());
                    s.WhenStarted(async tc => await tc.StartAsync());
                    s.WhenStopped(async tc => await tc.StopAsync());
                });

                x.RunAsLocalSystem();

                x.SetDescription("Slideshow Web Application");
                x.SetDisplayName("Slideshow App");
                x.SetServiceName("SlideshowApp");

            });

            var rc = host.Run();

            Environment.ExitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
        }

    }

}
