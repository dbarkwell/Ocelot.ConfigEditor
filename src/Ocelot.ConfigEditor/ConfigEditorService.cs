using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Configuration.Setter;
using Ocelot.Responses;

namespace Ocelot.ConfigEditor
{
    public class ConfigEditorService : IConfigEditorService
    {
        private const string ReloadFileName = "configeditor.reload";
        
        private readonly IFileConfigurationSetter _configSetter;
        private readonly IFileConfigurationRepository _fileConfigRepo;
        private readonly object _fileLock = new object();
        
        public ConfigEditorService(IFileConfigurationSetter configSetter, IFileConfigurationRepository fileConfigRepo)
        {
            _configSetter = configSetter;
            _fileConfigRepo = fileConfigRepo;
        }

        public bool IsReloadRequired()
        {
            return File.Exists(ReloadFileName);
        }

        public async Task ReloadConfigAsync()
        {
            var config = await GetConfigAsync();
            await SetConfigAsync(config.Data);
            
            RemoveReloadFlag();
        }

        public void AddReloadFlag()
        {
            lock (_fileLock)
            {
                if (File.Exists(ReloadFileName)) 
                    return;
                
                using (var fileStream = File.Create(ReloadFileName))
                {
                }
            }
        }

        public void RemoveReloadFlag()
        {
            lock (_fileLock)
            {
                if (File.Exists(ReloadFileName))
                    File.Delete(ReloadFileName);

            }
        }

        public async Task<Response<FileConfiguration>> GetConfigAsync()
        {
            return await _fileConfigRepo.Get();
        }

        public async Task SetConfigAsync(FileConfiguration fileConfig)
        {
            await _configSetter.Set(fileConfig);
        }
    }
}
