using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninject.Modules;
using Sentry.Protocol;
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
                .ConfigureLogging(config =>
                {
                    //config.
                });

            if (Environment.GetEnvironmentVariable("sentry__Dsn") != null)
            {
                builder.UseSentry(options =>
                {
                    // https://docs.sentry.io/platforms/dotnet/aspnetcore/
                    // dsn is configured only for production via K8S_SECRET_sentry__Dsn
                    // options.Dsn = "...";
                    options.MaxRequestBodySize = Sentry.Extensibility.RequestSize.Always;
                    options.SendDefaultPii = true;
                    options.MinimumBreadcrumbLevel = LogLevel.Debug;
                    options.MinimumEventLevel = LogLevel.Warning;
                    options.AttachStacktrace = true;
                    options.Debug = true;
                    options.DiagnosticsLevel = SentryLevel.Error;
                });
            }

            var host = builder.Build();

            this.Bind<IService>().To<WebHostService>()
                .InSingletonScope()
                .WithConstructorArgument("host", host);

            if (Environment.GetEnvironmentVariable("UNLEASH_API_URL") != null &&
                Environment.GetEnvironmentVariable("UNLEASH_INSTANCE_ID") != null
            )
            {
                this.Bind<IFeatureToggleProvider>()
                    .To<UnleashFeatureToggleProvider>()
                    .InSingletonScope()
                    .WithConstructorArgument("apiUrl", Environment.GetEnvironmentVariable("UNLEASH_API_URL"))
                    .WithConstructorArgument("instanceId", Environment.GetEnvironmentVariable("UNLEASH_INSTANCE_ID"));
            }
            else
            {
                this.Bind<IFeatureToggleProvider>().To<FakeFeatureToggleProvider>();
            }
            

            this.Bind<IDistributedCache>().To<DistributedCache>();
            this.Bind<IBackupProvider>().To<BackupProvider>();

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

            public string Name { get { return "WebHost"; } }

            public ServiceStatus Status { get; set; } = ServiceStatus.Stopped;

            public Task StartAsync()
            {
                if (this.Status != ServiceStatus.Stopped) throw new InvalidOperationException("Service is not stopped");
                try
                {
                    this.Status = ServiceStatus.StartPending;
                    host.RunAsync(cts.Token);
                    this.Status = ServiceStatus.Running;
                }
                catch (Exception)
                {
                    this.Status = ServiceStatus.Faulted;
                }
                return Task.CompletedTask;

            }

            public async Task StopAsync()
            {
                if (this.Status != ServiceStatus.Running) throw new InvalidOperationException("Service is not running");
                try
                {
                    this.Status = ServiceStatus.StopPending;
                    await host.StopAsync();
                    this.Status = ServiceStatus.Stopped;
                }
                catch (Exception)
                {
                    this.Status = ServiceStatus.Faulted;
                }
            }

        }

        private class FakeFeatureToggleProvider : IFeatureToggleProvider
        {
            public bool IsEnabled(string feature)
            {
                return true;
            }
        }
    }

}
