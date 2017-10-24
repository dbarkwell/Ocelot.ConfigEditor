using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Razor;

namespace Ocelot.ConfigEditor
{
    internal class ViewLocationMapper : IViewLocationExpander
    {
        private readonly IEnumerable<string> _preCompiledViewLocations;

        public ViewLocationMapper()
        {
            _preCompiledViewLocations = new[]
                                            {
                                                "/Editor/Views/{1}/{0}.cshtml", "/Editor/Views/Shared/{1}/{0}.cshtml",
                                                "/Editor/Views/Shared/{0}.cshtml", "/Editor/Views/{0}.cshtml"
                                            };
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            string isDashboardExample;
            if (context.Values.TryGetValue("OcelotConfigEditor", out isDashboardExample)
                && isDashboardExample == bool.TrueString) return _preCompiledViewLocations;

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.ActionContext.ActionDescriptor.MatchesNamespaceInRoute(context))
                context.Values.Add("OcelotConfigEditor", bool.TrueString);
        }
    }
}