using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ocelot.Configuration.File;

namespace Ocelot.ConfigEditor
{
    public interface IReloadService
    {
        Task<bool> IsReloadRequired();

        Task ReloadConfig();

        Task AddReloadFlag();

        Task RemoveReloadFlag();
    }
}
