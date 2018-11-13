using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Ocelot.ConfigEditor;
using Ocelot.ConfigEditor.Security;
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
                        var env = hostingContext.HostingEnvironment;
                        
                        config
                            .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                            .AddJsonFile("appsettings.json", true, true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                            .AddJsonFile("ocelot.json")
                            .AddEnvironmentVariables();

                        if (!env.IsDevelopment()) 
                            return;
                        
                        config.AddUserSecrets<TestSite>();
                    })
                .ConfigureServices(s =>
                    {
                        s.AddOcelot();
                        s.AddOcelotConfigEditor<GoogleAuthentication>();
                    })
                .ConfigureLogging((hostingContext, logging) =>
                    {
                        //add your logging
                    })
                .UseIISIntegration()
                .Configure(app =>
                    {
                        app.UseOcelotConfigEditor(new ConfigEditorOptions { Path = "edit" });
                        app.UseOcelot().Wait();
                    })
                .Build()
                .Run();
        }
    }
}
