using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Ocelot.ConfigEditor.Security;

namespace Ocelot.ConfigEditor
{
    public static class ConfigMiddlewareExtensions
    {
        public static IServiceCollection AddOcelotConfigEditor(this IServiceCollection services)
        {
            return services.AddOcelotConfigEditor<LocalhostAuthentication>();
        }
        
        public static IServiceCollection AddOcelotConfigEditor<T>(this IServiceCollection services) where T : Security.Authentication
        {
            var authSettings = new AuthorizeSettings();
            
            var auth = (T)Activator.CreateInstance(
                typeof(T), 
                services.BuildServiceProvider().GetService<IConfiguration>());
            
            authSettings.Name = auth.Name;
            authSettings.HasAuthentication = auth.EnforceHttps;
            
            auth.ConfigureServices(services);

            services
                .AddMvc(auth.SetupActions());

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
