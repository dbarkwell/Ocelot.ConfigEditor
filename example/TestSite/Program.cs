using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Ocelot.ConfigEditor;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace TestSite
{
    public class TestSite
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config
                            .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                            .AddJsonFile("appsettings.json", true, true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                            .AddJsonFile("ocelot.json")
                            .AddEnvironmentVariables();
                    })
                .ConfigureServices(s =>
                    {
                        s.AddOcelot();
                        s.AddOcelotConfigEditor();
                    })
                .ConfigureLogging((hostingContext, logging) =>
                    {
                        //add your logging
                    })
                .UseIISIntegration().Configure(app =>
                    {
                        app.UseOcelotConfigEditor(new ConfigEditorOptions { Path = "edit" });
                        app.UseOcelot().Wait();
                    })
                .Build()
                .Run();
        }
    }
}
