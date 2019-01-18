using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Ocelot.ConfigEditor;
using Ocelot.ConfigEditor.Security;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

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
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json", true, true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                            .AddJsonFile("ocelot.json", false, true)
                            .AddEnvironmentVariables();

                        if (!env.IsDevelopment()) 
                            return;
                        
                        config.AddUserSecrets<TestSite>();
                    })
                .ConfigureServices(s =>
                    {
                        s.AddOcelot();
                           // .AddConsul()
                          // .AddConfigStoredInConsul(); 
                        
                        s.AddOcelotConfigEditor<GoogleAuthentication>();
                    })
                .ConfigureLogging((hostingContext, logging) =>
                    {
                        //add your logging
                    })
                .UseIISIntegration()
                .Configure(app =>
                    {
                        app.UseOcelotConfigEditor();
                        app.UseOcelot().Wait();
                    })
                .Build()
                .Run();
        }
    }
}
