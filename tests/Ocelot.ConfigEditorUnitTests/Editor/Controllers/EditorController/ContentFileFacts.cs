using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;

using Ocelot.ConfigEditor;
using Ocelot.ConfigEditor.Editor.Controllers;
using Ocelot.Configuration.Repository;

using Xunit;

namespace Ocelot.ConfigEditorUnitTests.Editor.Controllers.EditorController
{
    public class ContentFileFacts
    {
        private readonly IFileConfigurationRepository _config;
        private readonly IConfigEditorService _configEditor;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _env;
        
        public ContentFileFacts()
        {
            _config = Substitute.For<IFileConfigurationRepository>();
            _configEditor = Substitute.For<IConfigEditorService>();
            _serviceProvider = Substitute.For<IServiceProvider>();
            _env = Substitute.For<IHostingEnvironment>();
        }

        [Fact]
        public void WhenRequestSiteCss_ReturnCssContentType()
        {
            var controller = new ConfigEditor.Editor.Controllers.EditorController(_configEditor, _serviceProvider, _env);

            var css = controller.ContentFile("site.min.css").ContentType;

            Assert.Equal("text/css", css);
        }

        [Fact]
        public void WhenRequestSiteCss_ReturnCssFile()
        {
            var controller = new ConfigEditor.Editor.Controllers.EditorController(_configEditor, _serviceProvider, _env);

            var fileStream = controller.ContentFile("site.min.css").FileStream;
            var css = string.Empty;

            using (var reader = new StreamReader(fileStream))
            {
                css = reader.ReadToEnd();
            }

            Assert.NotEqual(string.Empty, css);
        }

        [Fact]
        public void WhenRequestSiteJs_ReturnJsContentType()
        {
            var controller = new ConfigEditor.Editor.Controllers.EditorController(_configEditor, _serviceProvider, _env);

            var css = controller.ContentFile("site.min.js").ContentType;

            Assert.Equal("text/javascript", css);
        }

        [Fact]
        public void WhenRequestSiteJs_ReturnJsFile()
        {
            var controller = new ConfigEditor.Editor.Controllers.EditorController(_configEditor, _serviceProvider, _env);

            var fileStream = controller.ContentFile("site.min.js").FileStream;
            var css = string.Empty;

            using (var reader = new StreamReader(fileStream))
            {
                css = reader.ReadToEnd();
            }

            Assert.NotEqual(string.Empty, css);
        }
    }
}
