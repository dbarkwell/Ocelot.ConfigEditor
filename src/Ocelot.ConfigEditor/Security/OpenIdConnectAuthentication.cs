using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Ocelot.ConfigEditor.Security
{
    public class OpenIdConnectAuthentication : Authentication
    {
        public OpenIdConnectAuthentication(IConfiguration configuration, IHostingEnvironment environment) : base(configuration, environment)
        {
            SignOutSchemes = new[] { "Cookies", "OpenIdConnect" };
            CallbackPath = "/signin-oidc";
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = Configuration["OcelotConfigEditor:Authentication:OpenIdConnect:ClientId"];
                    options.ClientSecret = Configuration["OcelotConfigEditor:Authentication:OpenIdConnect:ClientSecret"];
                    options.Authority = Configuration["OcelotConfigEditor:Authentication:OpenIdConnect:Authority"];
                    options.CallbackPath = GetCallbackPath("OcelotConfigEditor:Authentication:OpenIdConnect:CallbackPath", CallbackPath);
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    // options.AccessDeniedPath = "/access-denied-from-remote";

                    options.ClaimActions.MapAllExcept("aud", "iss", "iat", "nbf", "exp", "aio", "c_hash", "uti", "nonce");

                    options.Events = new OpenIdConnectEvents()
                    {
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = 500;
                            context.Response.ContentType = "text/plain";
                            
                            return context.Response.WriteAsync(HostingEnvironment.IsDevelopment() ? 
                                context.Exception.ToString() : 
                                "An error occurred processing your authentication.");
                        }
                    };
                });
        }
    }
}