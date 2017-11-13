using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ConfigEditor
{
    public static class ConfigMiddlewareExtensions
    {
        public static IServiceCollection AddOcelotConfigEditor(this IServiceCollection services)
        {
            services.AddMvc();

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
            var reload = services.GetService<IReloadService>();
            reload.RemoveReloadFlag();

            var pathMatch = (configEditorOptions?.Path ?? "cfgedt").Trim('/');

            app.UseMvc(
                routes =>
                    {
                        routes.MapRoute(
                            "ConfigEditor",
                            $"{pathMatch}/{{controller=Editor}}/{{action=Index}}/{{id?}}",
                            null,
                            new { IsLocal = new LocalhostRouteConstraint() },
                            new { Namespace = "Ocelot.ConfigEditor.Editor.Controllers" });
                    });

            return app;
        }
    }
}
