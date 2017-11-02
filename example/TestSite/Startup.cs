using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CacheManager.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Ocelot.ConfigEditor;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace TestSite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            void Settings(ConfigurationBuilderCachePart x)
            {
                x.WithMicrosoftLogging(log => { log.AddConsole(LogLevel.Debug); }).WithDictionaryHandle();
            }

            services.AddOcelot(Configuration, Settings);
            services.AddOcelotConfigEditor();
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseOcelotConfigEditor(new ConfigEditorOptions { Path = "edit" });

            await app.UseOcelot();
        }
    }
}
