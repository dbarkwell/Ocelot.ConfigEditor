using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using Ocelot.ConfigEditor.Editor.Models;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;

namespace Ocelot.ConfigEditor.Editor.Controllers
{
    public class EditorController : Controller
    {
        private readonly IFileConfigurationRepository _fileConfigRepo;

        public EditorController(IFileConfigurationRepository fileConfigurationRepository)
        {
            _fileConfigRepo = fileConfigurationRepository;
        }

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

            return View("FileReRoute", viewModel);
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

            return RedirectToAction("Index");
        }

        [NamespaceConstraint]
        public IActionResult EditReRoute(string id)
        {
            var routes = _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route == null) return RedirectToAction("CreateReRoute");

            return View("FileReRoute", new FileReRouteViewModel { FileReRoute = route });
        }

        [NamespaceConstraint]
        public IActionResult Index()
        {
            var repo = _fileConfigRepo.Get();
            return View(repo.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NamespaceConstraint]
        public IActionResult SaveReRoute(string id, FileReRouteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("FileReRoute", model);
            }

            var routes = _fileConfigRepo.Get();
            var route = routes.Data.ReRoutes.FirstOrDefault(r => id == r.GetId());

            if (route != null) routes.Data.ReRoutes.Remove(route);

            routes.Data.ReRoutes.Add(model.FileReRoute);
            _fileConfigRepo.Set(routes.Data);

            return RedirectToAction("Index");
        }

        private static string GetMimeType(string fileId)
        {
            if (fileId.EndsWith(".js")) return "text/javascript";

            if (fileId.EndsWith(".css")) return "text/css";

            return fileId.EndsWith(".jpg") ? "image/jpeg" : "text";
        }
    }
}