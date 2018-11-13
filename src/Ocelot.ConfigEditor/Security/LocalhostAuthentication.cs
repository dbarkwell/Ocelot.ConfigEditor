using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.ConfigEditor.Filters;

namespace Ocelot.ConfigEditor.Security
{
    public class LocalhostAuthentication : Authentication
    {
        public LocalhostAuthentication(IConfiguration configuration) : base(configuration)
        {
            EnforceHttps = false;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
        }
        
        public override Action<MvcOptions> SetupActions()
        {
            return options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(context => context.HasSucceeded)
                    .Build();
                options.Filters.Add(new LocalhostAuthorizeFilter(policy));
            };
        }
    }
}