using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ocelot.Configuration.File;

namespace Ocelot.ConfigEditor
{
    public static class FileReRouteExtensions
    {
        public static string GetId(this FileReRoute fileReRoute)
        {
            return fileReRoute == null
                       ? string.Empty
                       : $"{fileReRoute.DownstreamScheme}" +
                         $"{GetHost(fileReRoute.DownstreamHostAndPorts)}" +
                         $"{GetPort(fileReRoute.DownstreamHostAndPorts)}" +
                         $"{fileReRoute.DownstreamPathTemplate}"
                           .Replace('/', '_');
        }

        private static string GetHost(IEnumerable<FileHostAndPort> hostAndPort)
        {
            var fileHostAndPorts = hostAndPort.ToList();
            return fileHostAndPorts.Any() ? fileHostAndPorts[0].Host : string.Empty;
        }

        private static int GetPort(IEnumerable<FileHostAndPort> hostAndPort)
        {
            var fileHostAndPorts = hostAndPort.ToList();
            return fileHostAndPorts.Any() ? fileHostAndPorts[0].Port : 0;            
        }
    }
}