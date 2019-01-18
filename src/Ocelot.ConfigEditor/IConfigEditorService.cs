using System.Threading.Tasks;

using Ocelot.Configuration.File;
using Ocelot.Responses;

namespace Ocelot.ConfigEditor
{
    public interface IConfigEditorService
    {
        bool IsReloadRequired();

        void AddReloadFlag();
        
        void RemoveReloadFlag();
        
        Task ReloadConfigAsync();

        Task<Response<FileConfiguration>> GetConfigAsync();

        Task SetConfigAsync(FileConfiguration fileConfig);
    }
}
