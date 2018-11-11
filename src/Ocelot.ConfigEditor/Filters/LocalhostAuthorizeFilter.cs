using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ocelot.ConfigEditor.Filters
{
    internal class LocalhostAuthorizeFilter : AuthorizeFilter, IAllowAnonymousFilter
    {
        public LocalhostAuthorizeFilter(AuthorizationPolicy policy) : base(policy) { }

        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            
            if (!CanAccessResources(httpContext))
                context.Result = new UnauthorizedResult();
            
            return Task.CompletedTask;
        }

        private static bool CanAccessResources(HttpContext httpContext)
        {
            if (httpContext.Connection.RemoteIpAddress == null)
                return false;

            if (httpContext.Connection.LocalIpAddress.Equals(IPAddress.Parse("127.0.0.1"))
                || httpContext.Connection.RemoteIpAddress.Equals(IPAddress.Parse("::1"))) 
                return true;

            return httpContext.Connection.LocalIpAddress.Equals(httpContext.Connection.RemoteIpAddress);
        }
    }
}