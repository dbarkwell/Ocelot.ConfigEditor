using System.IO;
using System.Threading.Tasks;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Configuration.Setter;
using Ocelot.Responses;

namespace Ocelot.ConfigEditor
{
    public class ConfigurationService : IConfigurationService
    {
        private const string ReloadFileName = "configeditor.reload";
        private readonly IFileConfigurationSetter _configSetter;
        private readonly IFileConfigurationRepository _fileConfigRepo;

        public ConfigurationService(IFileConfigurationSetter configSetter, IFileConfigurationRepository fileConfigRepo)
        {
            _configSetter = configSetter;
            _fileConfigRepo = fileConfigRepo;
        }

        public bool IsReloadRequired()
        {
            return File.Exists(ReloadFileName);
        }

        public async Task ReloadConfig()
        {
            var config = await GetConfig();
            await SetConfig(config.Data);
            
            RemoveReloadFlag();
        }

        public void AddReloadFlag()
        {
            using (var fileStream = File.Create(ReloadFileName))
            {
            }
        }

        public void RemoveReloadFlag()
        {
            if (File.Exists(ReloadFileName)) 
                File.Delete(ReloadFileName);
        }

        public async Task<Response<FileConfiguration>> GetConfig()
        {
            return await _fileConfigRepo.Get();
        }

        public async Task SetConfig(FileConfiguration fileConfig)
        {
            await _configSetter.Set(fileConfig);
        }
    }
}
