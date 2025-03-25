using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controllers;
using SolarWatch.Models;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Services.Repositories;

namespace SolarWatchTest
{
    [TestFixture]
    public class SunriseControllerTest
    {
        private Mock<ILogger<SunriseController>> _loggerMock;
        private Mock<IGeocodeProvider> _geocodeProviderMock;
        private Mock<IGeocodeJsonProcessor> _geocodeJsonProcessorMock;
        private Mock<ISolarTimeProvider> _solarTimeProviderMock;
        private Mock<ISolarTimeJsonProcessor> _solarTimeJsonProcessorMock;
        private Mock<ISolarTimeInfoRepository> _solarTimeInfoRepositoryMock;
        private Mock<ICityRepository> _cityRepositoryMock;
        private SunriseController _controller;
        private readonly string _cityInput = "Budapest,HU";
        private readonly DateOnly _dateInput = new(2023, 12, 01);
        private readonly string _tzidInput = "UTC";
        private readonly string _geocodeData = "[{}]";
        private readonly string _solarTimeData = "{}";

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<SunriseController>>();
            _geocodeProviderMock = new Mock<IGeocodeProvider>();
            _geocodeJsonProcessorMock = new Mock<IGeocodeJsonProcessor>();
            _solarTimeProviderMock = new Mock<ISolarTimeProvider>();
            _solarTimeJsonProcessorMock = new Mock<ISolarTimeJsonProcessor>();
            _solarTimeInfoRepositoryMock = new Mock<ISolarTimeInfoRepository>();
            _cityRepositoryMock = new Mock<ICityRepository>();
            _controller = new SunriseController(_loggerMock.Object, _geocodeProviderMock.Object, _geocodeJsonProcessorMock.Object,
                _solarTimeProviderMock.Object, _solarTimeJsonProcessorMock.Object, _solarTimeInfoRepositoryMock.Object, _cityRepositoryMock.Object);
        }

        [Test]
        public async Task GetSunriseTime_ReturnsBadRequest_IfGeocodeProviderFails()
        {
            _cityRepositoryMock.Setup(x => x.GetByName(_cityInput)).ReturnsAsync((City)null);
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ThrowsAsync(new HttpRequestException());

            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public async Task GetSunriseTime_ReturnsBadRequest_IfGeocodeJsonIsNotValid()
        {
            var nonExistingCity = "fdgbg,US";
            var errorMessage = "The city name and country code " +
                    "(and where it is needed state name) that you provided is not valid or cannot be found " +
                    "in the Geocoding API database!";
            _cityRepositoryMock.Setup(x => x.GetByName(nonExistingCity)).ReturnsAsync((City)null);
            _geocodeProviderMock.Setup(x => x.GetGeocode(nonExistingCity)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, nonExistingCity)).Throws(new Exception(errorMessage));

            var result = await _controller.GetSunriseTime(_dateInput, nonExistingCity, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.That(badRequest.Value, Is.EqualTo(errorMessage));
        }

        [Test]
        public async Task GetSunriseTime_ReturnsBadRequest_IfSolarTimeProviderFails()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_geocodes.lat, _geocodes.lon, _dateInput, _tzidInput)).
                ThrowsAsync(new Exception());

            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSunriseTimeReturnsBadRequestIfSolarTimeJsonIsNotValid()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_geocodes.lat, _geocodes.lon, _dateInput, _tzidInput)).
                ReturnsAsync(_solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessAndCreateSolarTimeInfo(_solarTimeData, _dateInput)).Throws<Exception>();

            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSunriseTimeReturnsOkResultIfSolarTimeJsonIsValid()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_geocodes.lat, _geocodes.lon, _dateInput, _tzidInput)).
                ReturnsAsync(_solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessAndCreateSolarTimeInfo(_solarTimeData, _dateInput)).Returns(_solarTimes);

            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                Assert.That(result.Value, Is.InstanceOf<TimeOnly>());
            });
        }
    }
}
