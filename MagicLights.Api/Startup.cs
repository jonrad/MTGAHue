using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ElectronNET.API;
using MagicLights.Api.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTGADispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Environment;

namespace MagicLights.Api
{
    public class Startup
    {
        private static readonly WindsorContainer container = new WindsorContainer();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            container.AddFacility<AspNetCoreFacility>(f => f.CrossWiresInto(services));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                })
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            RegisterApplicationComponents();

            return services.AddWindsor(container,
                opts => opts.UseEntryAssembly(typeof(ConfigurationController).Assembly),
                () => services.BuildServiceProvider(validateScopes: false));
        }

        private void RegisterApplicationComponents()
        {
            var installers = new List<IWindsorInstaller>();

            var game = new Game();

            installers.AddRange(
                new IWindsorInstaller[]
                {
                    new MagicDispatcherInstaller(MtgaOutputPath(), game),
                    new DebuggerInstaller(),
                    new ApplicationInstaller()
                });

            installers.Add(new HueInstaller());
            installers.Add(new ChromaInstaller());
            installers.Add(new CueInstaller());

            container.Install(installers.ToArray());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifeTime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Home}/{action=Index}/{id?}");
            });

            var application = container.Resolve<MagicLightsApplication>();

            var _ = application.Start();

            lifeTime.ApplicationStopped.Register(() =>
            {
                application.Stop();
            });

            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }

        private static string MtgaOutputPath()
        {
            var localPath = GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify);
            return Path.Combine(
                Path.GetDirectoryName(localPath),
                "LocalLow",
                "Wizards Of The Coast",
                "MTGA",
                "output_log.txt");
        }
    }
}
