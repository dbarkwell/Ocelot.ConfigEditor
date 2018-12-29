using System.Threading.Tasks;

using Ocelot.Configuration.File;
using Ocelot.Responses;

namespace Ocelot.ConfigEditor
{
    public interface IConfigurationService
    {
        bool IsReloadRequired();

        void AddReloadFlag();
        
        void RemoveReloadFlag();
        
        Task ReloadConfig();

        Task<Response<FileConfiguration>> GetConfig();

        Task SetConfig(FileConfiguration fileConfig);
    }
}
