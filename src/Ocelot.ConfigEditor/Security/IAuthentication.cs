using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ocelot.ConfigEditor.Security
{
    public interface IAuthentication
    {
        void ConfigureServices(IServiceCollection services);
    }
}