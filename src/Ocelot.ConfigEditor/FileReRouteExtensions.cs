using Ocelot.Configuration.File;

namespace Ocelot.ConfigEditor
{
    public static class FileReRouteExtensions
    {
        public static string GetId(this FileReRoute fileReRoute)
        {
            return fileReRoute == null
                       ? string.Empty
                       : $"{fileReRoute.DownstreamScheme}{fileReRoute.DownstreamHost}{fileReRoute.DownstreamPort}{fileReRoute.DownstreamPathTemplate}"
                           .Replace('/', '_');
        }
    }
}