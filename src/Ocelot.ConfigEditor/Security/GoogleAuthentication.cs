using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ConfigEditor.Security
{
    public class GoogleAuthentication : Authentication
    {
        public GoogleAuthentication(IConfiguration configuration, IHostingEnvironment environment) : base(configuration, environment)
        {
            SignOutSchemes = new [] { "Cookies" };
            CallbackPath = "/signin-google";
        }
        
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(
                    options =>
                    {    
                        options.ClientId = Configuration["OcelotConfigEditor:Authentication:Google:ClientId"];
                        options.ClientSecret = Configuration["OcelotConfigEditor:Authentication:Google:ClientSecret"];
                        options.CallbackPath = GetCallbackPath("OcelotConfigEditor:Authentication:Google:CallbackPath", CallbackPath);
                        options.SaveTokens = true;
                        options.Events = new OAuthEvents
                        {
                            OnRemoteFailure = context =>
                            {
                                context.HandleResponse();

                                context.Response.StatusCode = 500;
                                context.Response.ContentType = "text/plain";

                                return context.Response.WriteAsync(HostingEnvironment.IsDevelopment()
                                    ? context.Failure.ToString()
                                    : "An error occurred processing your authentication.");
                            }
                        };
                    });
        }
    }
}
