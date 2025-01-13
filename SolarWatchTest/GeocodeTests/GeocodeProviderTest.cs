using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.HttpClientWrapper;

namespace SolarWatchTest.GeocodeTests
{
    public class GeocodeProviderTest
    {
        private Mock<ILogger<GeocodeProvider>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IHttpClient> _httpClientMock;
        private GeocodeProvider _geocodeProvider;


        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<GeocodeProvider>>();
            _configurationMock = new Mock<IConfiguration>();
            _httpClientMock = new Mock<IHttpClient>();
            _geocodeProvider = new GeocodeProvider(_loggerMock.Object, _configurationMock.Object, _httpClientMock.Object);

        }

        [Test]
        public async Task GetGeocodeThrowsExceptionIfApiCallIsNotSuccessful()
        {
            var cityInput = "London";
            var fakeApiKey = "ApiKey";
            _configurationMock.Setup(x => x["ApiKeys:OpenWeatherMap"]).Returns(fakeApiKey);
            _httpClientMock.Setup(x => x.GetStringAsync(It.Is<string>(url => url.Contains(cityInput) && url.Contains(fakeApiKey)))).ThrowsAsync(new Exception());

            var result = Assert.ThrowsAsync<Exception>(async () => await _geocodeProvider.GetGeocode(cityInput));

            Assert.That(result, Is.InstanceOf<Exception>());
        }

        [Test]
        public async Task GetGeocodeReturnsGeocodeInfoIfApiCallIsSuccessful()
        {
            var cityInput = "London";
            var fakeApiKey = "ApiKey";
            var fakeResponse = "[{\"name\":\"London\",\"lat\":51.5074,\"lon\":-0.1278}]";
            _configurationMock.Setup(x => x["ApiKeys:OpenWeatherMap"]).Returns(fakeApiKey);
            _httpClientMock.Setup(x => x.GetStringAsync(It.Is<string>(url => url.Contains(cityInput) && url.Contains(fakeApiKey)))).ReturnsAsync(fakeResponse);

            var result = await _geocodeProvider.GetGeocode(cityInput);

            Assert.That(result, Is.EqualTo(fakeResponse));
        }
    }
}