using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ConfigEditor.Security
{
    public abstract class Authentication
    {
        protected Authentication(IConfiguration configuration)
        {
            Configuration = configuration;
            Name = this.GetType().FullName;
        }
        
        public string Name { get; protected set; }

        public bool EnforceHttps { get; protected set; } = true;
        
        public abstract void ConfigureServices(IServiceCollection services);

        protected IConfiguration Configuration { get; }
        
        public virtual Action<MvcOptions> SetupActions()
        {
            return options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            };
        }
    }
}