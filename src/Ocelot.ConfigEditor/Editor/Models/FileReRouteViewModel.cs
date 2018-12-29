using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using Ocelot.Configuration.File;

namespace Ocelot.ConfigEditor.Editor.Models
{
    public class FileReRouteViewModel
    {
        public FileReRoute FileReRoute { get; set; }
        
        public IEnumerable<SelectListItem> HttpMethodListItems => new List<SelectListItem>
                                                                      {
                                                                          new SelectListItem
                                                                              {
                                                                                  Text
                                                                                      = "Get",
                                                                                  Value
                                                                                      = "Get"
                                                                              },
                                                                          new SelectListItem
                                                                              {
                                                                                  Text
                                                                                      = "Post",
                                                                                  Value
                                                                                      = "Post"
                                                                              },
                                                                          new SelectListItem
                                                                              {
                                                                                  Text
                                                                                      = "Put",
                                                                                  Value
                                                                                      = "Put"
                                                                              },
                                                                          new SelectListItem
                                                                              {
                                                                                  Text
                                                                                      = "Patch",
                                                                                  Value
                                                                                      = "Patch"
                                                                              },
                                                                          new SelectListItem
                                                                              {
                                                                                  Text
                                                                                      = "Delete",
                                                                                  Value
                                                                                      = "Delete"
                                                                              }
                                                                      };

        public IEnumerable<SelectListItem> SchemeListItems => new List<SelectListItem>
                                                                  {
                                                                      new SelectListItem
                                                                          {
                                                                              Text =
                                                                                  "Http",
                                                                              Value
                                                                                  = "http"
                                                                          },
                                                                      new SelectListItem
                                                                          {
                                                                              Text =
                                                                                  "Https",
                                                                              Value
                                                                                  = "https"
                                                                          }
                                                                  };
    }
}