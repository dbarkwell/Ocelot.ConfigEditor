using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using Ocelot.ConfigEditor.Editor.Models;
using Ocelot.Configuration;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Provider;
using Ocelot.Configuration.Repository;
using Ocelot.Configuration.Setter;

namespace Ocelot.ConfigEditor.Editor.Controllers
{
    public class EditorController : Controller
    {
        private readonly IFileConfigurationRepository _fileConfigRepo;

        private readonly IReloadService _reload;

        public EditorController(IFileConfigurationRepository fileConfigurationRepository, IReloadService reload)
        {
            _fileConfigRepo = fileConfigurationRepository;
            _reload = reload;
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
        public IActionResult CreateReRoute(string id, FileReRouteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var validator = new FileReRouteValidator();
            var results = validator.Validate(model.FileReRoute);

            if (!results.IsValid)
            {
                results.Errors.ToList().ForEach(e => ModelState.AddModelError($"FileReRoute.{e.PropertyName}", e.ErrorMessage));
                return View(model);
            }

            var routes = _fileConfigRepo.Get();
            routes.Data.ReRoutes.Add(model.FileReRoute);
            _fileConfigRepo.Set(routes.Data);

            _reload.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public IActionResult DeleteReRoute(string id)
        {
            var routes = _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route == null) return RedirectToAction("Index");

            routes.Data.ReRoutes.Remove(route);
            _fileConfigRepo.Set(routes.Data);

            _reload.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [NamespaceConstraint]
        public IActionResult EditReRoute(string id)
        {
            var routes = _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route == null) return RedirectToAction("CreateReRoute");

            return View(new FileReRouteViewModel { FileReRoute = route });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public IActionResult EditReRoute(string id, FileReRouteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var validator = new FileReRouteValidator();
            var results = validator.Validate(model.FileReRoute);

            if (!results.IsValid)
            {
                results.Errors.ToList().ForEach(e => ModelState.AddModelError($"FileReRoute.{e.PropertyName}", e.ErrorMessage));
                return View(model);
            }

            var routes = _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route != null) routes.Data.ReRoutes.Remove(route);

            routes.Data.ReRoutes.Add(model.FileReRoute);
            _fileConfigRepo.Set(routes.Data);

            _reload.AddReloadFlag();

            return RedirectToAction("Index");
        }

        [NamespaceConstraint]
        public IActionResult Index()
        {
            var repo = _fileConfigRepo.Get();
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
    }
}