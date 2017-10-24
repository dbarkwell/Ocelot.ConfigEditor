using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ocelot.ConfigEditor
{
    internal class LocalhostRouteConstraint : IRouteConstraint
    {
        public bool Match(
            HttpContext httpContext,
            IRouter router,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (httpContext.Connection.RemoteIpAddress == null) return false;

            if (httpContext.Connection.LocalIpAddress.Equals(IPAddress.Parse("127.0.0.1"))
                || httpContext.Connection.RemoteIpAddress.Equals(IPAddress.Parse("::1"))) return true;

            return httpContext.Connection.LocalIpAddress.Equals(httpContext.Connection.RemoteIpAddress);
        }
    }
}