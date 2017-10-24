using Ocelot.ConfigEditor;
using Ocelot.Configuration.File;

using Xunit;

namespace Ocelot.ConfigEditorUnitTests.FileReRouteExtensions
{
    public class GetIdFacts
    {
        [Fact]
        public void WhenGivenFileReRoute_ReturnId()
        {
            var fileReRoute = new FileReRoute
                                  {
                                      DownstreamScheme = "http",
                                      DownstreamHost = "localhost",
                                      DownstreamPort = 80,
                                      DownstreamPathTemplate = "/test"
                                  };

            var id = fileReRoute.GetId();

            Assert.Equal("httplocalhost80_test", id);
        }

        [Fact]
        public void WhenGivenNullFileReRoute_ReturnEmptyString()
        {
            FileReRoute fileReRoute = null;

            var id = fileReRoute.GetId();

            Assert.Equal(string.Empty, id);
        }

        [Fact]
        public void WhenMissingPortInFileReRoute_Return0Port()
        {
            var fileReRoute = new FileReRoute
                                  {
                                      DownstreamScheme = "http",
                                      DownstreamHost = "localhost",
                                      DownstreamPathTemplate = "/test"
                                  };

            var id = fileReRoute.GetId();

            Assert.Equal("httplocalhost0_test", id);
        }
    }
}