using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Configuration.Setter;

namespace Ocelot.ConfigEditor
{
    public class ReloadService : IReloadService
    {
        private const string ReloadFileName = "configeditor.reload";
        private readonly IFileConfigurationSetter _configSetter;
        private readonly IFileConfigurationRepository _fileConfigRepo;

        public ReloadService(IFileConfigurationSetter configSetter, IFileConfigurationRepository fileConfigRepo)
        {
            _configSetter = configSetter;
            _fileConfigRepo = fileConfigRepo;
        }

        public async Task<bool> IsReloadRequired()
        {
            return await Task.Run(() => File.Exists(ReloadFileName));
        }

        public async Task ReloadConfig()
        {
            await Task.Run(
                () =>
                    {
                        var config = _fileConfigRepo.Get();
                        _configSetter.Set(config.Data);
                    });

            await RemoveReloadFlag();
        }

        public async Task AddReloadFlag()
        {
            await Task.Run(
                () =>
                    {
                        using (var fileStream = File.Create(ReloadFileName))
                        {
                        }
                    });
        }

        public async Task RemoveReloadFlag()
        {
            await Task.Run(
                () =>
                    {
                        if (File.Exists(ReloadFileName)) File.Delete(ReloadFileName);
                    });
        }
    }
}
