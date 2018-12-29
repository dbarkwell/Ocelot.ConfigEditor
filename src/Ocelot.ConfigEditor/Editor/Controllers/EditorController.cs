using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FluentValidation.Results;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

using Ocelot.ConfigEditor.Editor.Models;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Validator;

namespace Ocelot.ConfigEditor.Editor.Controllers
{
    [Authorize]
    public class EditorController : Controller
    {
        private const long MaxFileUploadSize = 1024 * 1024 * 28;
        private readonly IConfigurationService _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _env;
        
        public EditorController(
            IConfigurationService configuration, 
            IServiceProvider serviceProvider, 
            IHostingEnvironment env)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _env = env;
        }

        [NamespaceConstraint]
        public FileStreamResult ContentFile(string id)
        {
            var assembly = typeof(EditorController).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(f => f.EndsWith(id));
            var resource = assembly.GetManifestResourceStream(resourceName);
            return new FileStreamResult(resource, GetMimeType(id));
        }

        [NamespaceConstraint]
        public IActionResult CreateReRoute()
        {
            var viewModel = new FileReRouteViewModel { FileReRoute = new FileReRoute() };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public async Task<IActionResult> CreateReRoute(string id, FileReRouteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var results = ValidateModel(model);
            if (!results.IsValid)
            {
                results.Errors.ToList().ForEach(e => ModelState.AddModelError($"FileReRoute.{e.PropertyName}", e.ErrorMessage));
                return View(model);
            }

            var routes = await _configuration.GetConfig();
            routes.Data.ReRoutes.Add(model.FileReRoute);
            await _configuration.SetConfig(routes.Data);

            _configuration.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public async Task<IActionResult> DeleteReRoute(string id)
        {
            var routes = await _configuration.GetConfig();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route == null) return RedirectToAction("Index");

            routes.Data.ReRoutes.Remove(route);
            await _configuration.SetConfig(routes.Data);

            _configuration.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [NamespaceConstraint]
        public async Task<IActionResult> EditReRoute(string id)
        {
            var routes = await _configuration.GetConfig();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route == null) return RedirectToAction("CreateReRoute");

            return View(new FileReRouteViewModel { FileReRoute = route });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public async Task<IActionResult> EditReRoute(string id, FileReRouteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var results = ValidateModel(model);
            if (!results.IsValid)
            {
                results.Errors.ToList().ForEach(e => ModelState.AddModelError($"FileReRoute.{e.PropertyName}", e.ErrorMessage));
                return View(model);
            }

            var routes = await _configuration.GetConfig();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route != null) 
                routes.Data.ReRoutes.Remove(route);

            routes.Data.ReRoutes.Add(model.FileReRoute);
            await _configuration.SetConfig(routes.Data);
            
            _configuration.AddReloadFlag();

            return RedirectToAction("Index");
        }
        
        [NamespaceConstraint]
        public async Task<IActionResult> DownloadRoutes(bool minified = false)
        {
            const string mimeType = "application/text";
            var config = await _configuration.GetConfig();
            var jsonFormatting = minified ? Formatting.None : Formatting.Indented;
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore, ContractResolver = new IgnoreEmptyEnumerableResolver()};
            var contents = JsonConvert.SerializeObject(config.Data, jsonFormatting, settings);
            
            return File(Encoding.UTF8.GetBytes(contents), mimeType, "ocelot.json");
        }

        [HttpGet]
        [NamespaceConstraint]
        public IActionResult UploadRoutes()
        {
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [NamespaceConstraint]
        public async Task<IActionResult> UploadRoutes(IFormFile configFile)
        {
            try
            {
                var size = configFile.Length;
                if (size > MaxFileUploadSize)
                    throw new Exception("Config file is too large.");
                
                var serializer = new JsonSerializer();
                
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await configFile.CopyToAsync(stream);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        var jsonReader = new JsonTextReader(reader);
                        var config = serializer.Deserialize<FileConfiguration>(jsonReader);
                        if (config == null)
                            throw new Exception("Config file is empty.");
                        
                        await _configuration.SetConfig(config);
                    }
                } 
            }
            catch (Exception e)
            {
                return View("Index", await GetIndexViewModel($"Unable to parse {configFile.FileName}. Please review and try again. {e.Message}"));
            }
            
            await ReloadConfig();
            
            return RedirectToAction("Index");
        }
        
        [NamespaceConstraint]
        public async Task<IActionResult> Index(IndexViewModel model = null)
        {
            return View(await GetIndexViewModel());
        }

        [HttpPost]
        [NamespaceConstraint]
        public async Task<IActionResult> Reload()
        {
            await ReloadConfig();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [NamespaceConstraint]
        public async Task<IActionResult> SignOut()
        {
            var authSettings = _serviceProvider.GetService<AuthorizeSettings>();
            foreach (var scheme in authSettings.SignOutSchemes)
            {
                await HttpContext.SignOutAsync(scheme);
            }
            
            return Redirect("/");
        }
        
        [AllowAnonymous]
        [NamespaceConstraint]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task ReloadConfig()
        {
            await _configuration.ReloadConfig();
        }
        
        private static string GetMimeType(string fileId)
        {
            if (fileId.EndsWith(".js")) return "text/javascript";

            if (fileId.EndsWith(".css")) return "text/css";

            if (fileId.EndsWith(".eot")) return "application/vnd.ms-fontobject";

            if (fileId.EndsWith(".ttf")) return "application/font-sfnt";

            if (fileId.EndsWith(".svg")) return "image/svg+xml";

            if (fileId.EndsWith(".woff")) return "application/font-woff";

            if (fileId.EndsWith(".woff2")) return "application/font-woff2";

            return fileId.EndsWith(".jpg") ? "image/jpeg" : "text";
        }

        private ValidationResult ValidateModel(FileReRouteViewModel model)
        {
            var hostAndPortValidator = new HostAndPortValidator();
            var fileQoSValidator = new FileQoSOptionsFluentValidator(_serviceProvider);
            var validator = new ReRouteFluentValidator(null, hostAndPortValidator, fileQoSValidator);
            return validator.Validate(model.FileReRoute);
        }
        
        private async Task<IndexViewModel> GetIndexViewModel()
        {
            var repo = await _configuration.GetConfig();
            var model = new IndexViewModel {FileConfiguration = repo.Data};
            return model;
        }
        
        private async Task<IndexViewModel> GetIndexViewModel(string errorMessage)
        {
            var model = await GetIndexViewModel();
            model.Error.ErrorMessage = errorMessage;
            return model;
        }

    }
}