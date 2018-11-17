using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ConfigEditor.Security
{
    public abstract class Authentication
    {
        protected Authentication(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
            Name = this.GetType().FullName;
        }
        
        public string Name { get; protected set; }

        public bool EnforceHttps { get; protected set; } = true;
        
        public IEnumerable<string> SignOutSchemes { get; protected set; }
        
        public abstract void ConfigureServices(IServiceCollection services);

        protected IConfiguration Configuration { get; }
        
        protected IHostingEnvironment HostingEnvironment { get; }
        
        protected string CallbackPath { get; set; }
        
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
        
        protected string GetCallbackPath(string key, string callbackPath)
        {
            var configCallbackPath = Configuration[key];
            if (configCallbackPath == null &&
                Configuration["OcelotConfigEditor:Path"] != null)
            {
                return $"/{Configuration["OcelotConfigEditor:Path"]}{callbackPath}";
            }

            return configCallbackPath;
        }
    }
}