using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using slideshow.core;
using slideshow.core.Repository;
using System;
using System.Linq;
using System.Threading;

namespace slideshow
{
    public class Startup
    {

        // shttps://dev.to/cwetanow/wiring-up-ninject-with-aspnet-core-20-3hp

        private readonly AsyncLocal<Scope> scopeProvider = new AsyncLocal<Scope>();
        private IKernel kernel { get; set; }

        private object Resolve(Type type) => kernel.Get(type);
        private object RequestScope(IContext context) => scopeProvider.Value;

        private sealed class Scope : DisposableObject { }

        public Startup(IConfiguration configuration, IKernel kernel)
        {
            Configuration = configuration;
            this.kernel = kernel;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // https://stackoverflow.com/questions/36095076/custom-authentication-in-asp-net-core
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });

            services.AddSingleton<IDistributedCache, DistributedCache>();

            Func<ICacheEntryRepository> factory = () => kernel.Get<ICacheEntryRepository>();
            services.AddDataProtection(config =>
            {
                config.ApplicationDiscriminator = "slideshow";
            }).PersistKeysToDb(factory);
            
            //services.AddDistributedCache();

            services.AddMvc(config =>
            {
                // disabled, see #13
                // config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                config.RespectBrowserAcceptHeader = true;
                config.InputFormatters.Add(new XmlSerializerInputFormatter(config));
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                //config.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
            })
            .AddXmlSerializerFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Other configurations

            // authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRequestScopingMiddleware(() => scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);

            services.AddTransient<ISectionRepository>(provider => kernel.Get<ISectionRepository>());
            services.AddTransient<ISlideRepository>(provider => kernel.Get<ISlideRepository>());
            services.AddTransient<ICacheEntryRepository>(provider => kernel.Get<ICacheEntryRepository>());
            //services.AddTransient<IDistributedCache>(provider => kernel.Get<IDistributedCache>());
            services.AddSingleton<IFeatureToggleProvider>(provider => kernel.Get<IFeatureToggleProvider>());
            //return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.RegisterApplicationComponents(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if ((Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_REDIRECTION_ENABLED") ?? "true") == "true")
            {
                // https redirection does not work with gitpod
                // allow disable it
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private IKernel RegisterApplicationComponents(IApplicationBuilder app)
        {
            // IKernelConfiguration config = new KernelConfiguration();
            //var kernel = new StandardKernel();

            // Register application services
            foreach (var ctrlType in app.GetControllerTypes())
            {
                kernel.Bind(ctrlType).ToSelf().InScope(RequestScope);
            }

            // This is where our bindings are configurated
            //kernel.Bind<ITestService>().To<TestService>().InScope(RequestScope);
            kernel.Bind<IOutputFormatter>().To<XmlSerializerOutputFormatter>();


            // Cross-wire required framework services
            kernel.BindToMethod(app.GetRequestService<IViewBufferScope>);

            //kernel.Bind<IDistributedCache>().To<DistributedCache>();


            return kernel;
        }
    }

    public sealed class RequestScopingStartupFilter : IStartupFilter
    {
        private readonly Func<IDisposable> requestScopeProvider;

        public RequestScopingStartupFilter(Func<IDisposable> requestScopeProvider)
        {
            if (requestScopeProvider == null)
            {
                throw new ArgumentNullException(nameof(requestScopeProvider));
            }

            this.requestScopeProvider = requestScopeProvider;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextFilter)
        {
            return builder =>
            {
                ConfigureRequestScoping(builder);

                nextFilter(builder);
            };
        }

        private void ConfigureRequestScoping(IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                using (var scope = this.requestScopeProvider())
                {
                    await next();
                }
            });
        }
    }

    public static class AspNetCoreExtensions
    {
        public static void AddRequestScopingMiddleware(this IServiceCollection services,
            Func<IDisposable> requestScopeProvider)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (requestScopeProvider == null)
            {
                throw new ArgumentNullException(nameof(requestScopeProvider));
            }

            services
                .AddSingleton<IStartupFilter>(new
                    RequestScopingStartupFilter(requestScopeProvider));
        }

        public static void AddCustomControllerActivation(this IServiceCollection services,
        Func<Type, object> activator)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (activator == null) throw new ArgumentNullException(nameof(activator));

            services.AddSingleton<IControllerActivator>(new DelegatingControllerActivator(
                context => activator(context.ActionDescriptor.ControllerTypeInfo.AsType())));
        }

        public static void AddCustomViewComponentActivation(this IServiceCollection services,
     Func<Type, object> activator)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (activator == null) throw new ArgumentNullException(nameof(activator));

            services.AddSingleton<IViewComponentActivator>(
new DelegatingViewComponentActivator(activator));
        }
    }

    public sealed class DelegatingControllerActivator : IControllerActivator
    {
        private readonly Func<ControllerContext, object> controllerCreator;
        private readonly Action<ControllerContext, object> controllerReleaser;

        public DelegatingControllerActivator(Func<ControllerContext, object> controllerCreator,
            Action<ControllerContext, object> controllerReleaser = null)
        {
            this.controllerCreator = controllerCreator ??
                throw new ArgumentNullException(nameof(controllerCreator));
            this.controllerReleaser = controllerReleaser ?? ((_, __) => { });
        }

        public object Create(ControllerContext context) => this.controllerCreator(context);
        public void Release(ControllerContext context, object controller) =>
            this.controllerReleaser(context, controller);
    }

    public sealed class DelegatingViewComponentActivator : IViewComponentActivator
    {
        private readonly Func<Type, object> viewComponentCreator;
        private readonly Action<object> viewComponentReleaser;

        public DelegatingViewComponentActivator(Func<Type, object> viewComponentCreator,
            Action<object> viewComponentReleaser = null)
        {
            this.viewComponentCreator = viewComponentCreator ??
                throw new ArgumentNullException(nameof(viewComponentCreator));
            this.viewComponentReleaser = viewComponentReleaser ?? (_ => { });
        }

        public object Create(ViewComponentContext context) =>
            this.viewComponentCreator(context.ViewComponentDescriptor.TypeInfo.AsType());

        public void Release(ViewComponentContext context, object viewComponent) =>
            this.viewComponentReleaser(viewComponent);
    }

    public static class ApplicationBuilderExtensions
    {
        public static void BindToMethod<T>(this IKernel config, Func<T> method)
 => config.Bind<T>().ToMethod(c => method());

        public static Type[] GetControllerTypes(this IApplicationBuilder builder)
        {
            var manager = builder.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);

            return feature.Controllers.Select(t => t.AsType()).ToArray();
        }

        public static T GetRequestService<T>(this IApplicationBuilder builder) where T : class
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return GetRequestServiceProvider(builder).GetService<T>();
        }

        private static IServiceProvider GetRequestServiceProvider(IApplicationBuilder builder)
        {
            var accessor = builder.ApplicationServices.GetService<IHttpContextAccessor>();

            if (accessor == null)
            {
                throw new InvalidOperationException(
          typeof(IHttpContextAccessor).FullName);
            }

            var context = accessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("No HttpContext.");
            }

            return context.RequestServices;
        }
    }

}
