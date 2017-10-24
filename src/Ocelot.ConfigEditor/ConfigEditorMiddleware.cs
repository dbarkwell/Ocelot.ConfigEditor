using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

using Ocelot.ConfigEditor;

namespace Microsoft.AspNetCore.Builder
{
    public static class ConfigMiddleware
    {
        public static IServiceCollection AddOcelotConfigEditor(this IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<RazorViewEngineOptions>(
                opt => { opt.ViewLocationExpanders.Add(new ViewLocationMapper()); });

            return services;
        }

        public static IApplicationBuilder UseOcelotConfigEditor(
            this IApplicationBuilder app,
            ConfigEditorOptions configEditorOptions = null)
        {
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