using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ConfigEditor.Security
{
    // ReSharper disable once InconsistentNaming
    public class AzureADAuthentication : Authentication
    {
        private const string Instance = "https://login.microsoftonline.com/";
        
        public AzureADAuthentication(IConfiguration configuration, IHostingEnvironment environment) : base(configuration, environment)
        {
            SignOutSchemes = new[] { "Cookies", "AzureAD", "AzureADOpenID", "AzureADCookie" };
            CallbackPath = "/signin-oidc";
        }
        
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddCookie()
                .AddAzureAD(
                    options =>
                    {
                        options.Instance = Instance;
                        options.ClientId = Configuration["OcelotConfigEditor:Authentication:AzureAd:ClientId"];
                        options.Domain = Configuration["OcelotConfigEditor:Authentication:AzureAd:Domain"];
                        options.TenantId = Configuration["OcelotConfigEditor:Authentication:AzureAd:TenantId"];
                        options.CallbackPath = GetCallbackPath("OcelotConfigEditor:Authentication:AzureAd:CallbackPath", CallbackPath);
                    });
        }
    }
}
