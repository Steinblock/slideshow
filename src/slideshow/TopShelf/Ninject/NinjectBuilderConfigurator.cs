using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topshelf.Builders;
using Topshelf.Configurators;
using Topshelf.HostConfigurators;

namespace TopShelf.Ninject
{
    public class NinjectBuilderConfigurator : HostBuilderConfigurator
    {
        private static INinjectSettings _settings;
        private static INinjectModule[] _modules;
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get
            {
                if (_kernel == null)
                    _kernel = _settings != null ? new StandardKernel(_settings, _modules) : new StandardKernel(_modules);
                return _kernel;

            }
        }

        public NinjectBuilderConfigurator(INinjectSettings settings, INinjectModule[] modules)
        {
            _settings = settings;
            _modules = modules;
        }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }
    }
}
