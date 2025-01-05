using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.WebClientWrapper;

namespace SolarWatchTest.GeocodeTests
{
    public class GeocodeProviderTest
    {
        private Mock<ILogger<GeocodeProvider>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IWebClient> _webClientMock;
        private GeocodeProvider _geocodeProvider;


        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<GeocodeProvider>>();
            _configurationMock = new Mock<IConfiguration>();
            _geocodeProvider = new GeocodeProvider(_loggerMock.Object, _configurationMock.Object, _webClientMock.Object);

        }

        [Test]
        public void Test1()
        {

            Assert.Pass();
        }
    }
}