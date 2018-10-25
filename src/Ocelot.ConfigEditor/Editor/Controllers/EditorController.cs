using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

using Ocelot.ConfigEditor.Editor.Models;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Configuration.Validator;

namespace Ocelot.ConfigEditor.Editor.Controllers
{
    public class EditorController : Controller
    {
        private readonly IFileConfigurationRepository _fileConfigRepo;

        private readonly IReloadService _reload;

        private readonly IServiceProvider _serviceProvider;

        public EditorController(IFileConfigurationRepository fileConfigurationRepository, IReloadService reload, IServiceProvider serviceProvider)
        {
            _fileConfigRepo = fileConfigurationRepository;
            _reload = reload;
            _serviceProvider = serviceProvider;
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

            var routes = await _fileConfigRepo.Get();
            routes.Data.ReRoutes.Add(model.FileReRoute);
            await _fileConfigRepo.Set(routes.Data);

            await _reload.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public async Task<IActionResult> DeleteReRoute(string id)
        {
            var routes = await _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route == null) return RedirectToAction("Index");

            routes.Data.ReRoutes.Remove(route);
            await _fileConfigRepo.Set(routes.Data);

            await _reload.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [NamespaceConstraint]
        public async Task<IActionResult> EditReRoute(string id)
        {
            var routes = await _fileConfigRepo.Get();
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

            var routes = await _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route != null) 
                routes.Data.ReRoutes.Remove(route);

            routes.Data.ReRoutes.Add(model.FileReRoute);
            var response = await _fileConfigRepo.Set(routes.Data);
            
            await _reload.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [NamespaceConstraint]
        public async Task<IActionResult> Index()
        {
            var repo = await _fileConfigRepo.Get();
            return View(repo.Data);
        }

        [HttpPost]
        [NamespaceConstraint]
        public IActionResult Reload()
        {
            _reload.ReloadConfig();

            return RedirectToAction("Index");
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
    }
}