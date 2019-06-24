using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ninject;
using Sentry;
using slideshow.db;
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
            using (SentrySdk.Init("https://67eb60e7372c4ab78eec333390b63c31@sentry.io/1488570"))
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

                if (args.Any(x => x == "--migrate"))
                {
                    Console.WriteLine("Try migrate database");
                    var context = kernel.Get<SlideshowContext>();
                    context.Database.Migrate();
                    Console.WriteLine("Done");
                    Environment.Exit(0);
                }

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

}
