using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

using Ocelot.ConfigEditor.Filters;
using Ocelot.ConfigEditor.Security;

namespace Ocelot.ConfigEditor
{
    public static class ConfigMiddlewareExtensions
    {
        public static IServiceCollection AddOcelotConfigEditor(this IServiceCollection services, IAuthentication auth = null)
        {
            var authSettings = new AuthorizeSettings();
            
            if (auth == null)
            {
                authSettings.Name = "LocalhostAuthorizeFilter";
                
                services.AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAssertion(context => context.HasSucceeded)
                        .Build();
                    options.Filters.Add(new LocalhostAuthorizeFilter(policy));
                });
            }
            else
            {
                authSettings.Name = auth.GetType().FullName;
                authSettings.HasAuthentication = true;
                
                auth.ConfigureServices(services);
            }

            services.AddSingleton(authSettings);
            
            services.Configure<RazorViewEngineOptions>(
                opt => { opt.ViewLocationExpanders.Add(new ViewLocationMapper()); });

            services.AddScoped<IReloadService, ReloadService>();
            
            return services;
        }
        
        public static IApplicationBuilder UseOcelotConfigEditor(
            this IApplicationBuilder app,
            ConfigEditorOptions configEditorOptions = null)
        {
            var services = app.ApplicationServices;
            var env = services.GetService<IHostingEnvironment>();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Editor/Error");
                app.UseHsts();
            }
            
            var reload = services.GetService<IReloadService>();
            reload.RemoveReloadFlag();
            
            var pathMatch = (configEditorOptions?.Path ?? "cfgedt").Trim('/');
            
            var authSettings = services.GetService<AuthorizeSettings>();
            if (authSettings.HasAuthentication)
            {
                app.UseWhen(context => context.Request.Path.StartsWithSegments($"/{pathMatch}"),
                    appBuilder =>
                    {
                        appBuilder.UseHttpsRedirection();
                        appBuilder.UseAuthentication();
                    });
            }
            
            app.UseMvc(
                routes =>
                    {
                        routes.MapRoute(
                            "ConfigEditor",
                            $"{pathMatch}/{{controller=Editor}}/{{action=Index}}/{{id?}}",
                            null,
                            null,
                            new { Namespace = "Ocelot.ConfigEditor.Editor.Controllers" });
                    });

            return app;
        }
    }
}
