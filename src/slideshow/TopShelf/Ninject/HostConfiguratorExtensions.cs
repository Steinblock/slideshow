using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topshelf.HostConfigurators;
using Topshelf.Logging;

namespace TopShelf.Ninject
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseNinject(this HostConfigurator configurator, params INinjectModule[] modules)
        {
            return UseNinject(configurator, null, modules);
        }

        public static HostConfigurator UseNinject(this HostConfigurator configurator, INinjectSettings settings, params INinjectModule[] modules)
        {
            var log = HostLogger.Get(typeof(HostConfiguratorExtensions));

            log.Info("[Topshelf.Ninject] Integration Started in host.");
            log.Debug(string.Format("Ninject kernel instantiated with {0} modules.", modules.Length));

            configurator.AddConfigurator(new NinjectBuilderConfigurator(settings, modules));
            return configurator;
        }
    }
}
