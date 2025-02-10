using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controllers;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.JsonProcessors;

namespace SolarWatchTest
{
    [TestFixture]
    public class SunsetControllerTest
    {
        private Mock<ILogger<SunsetController>> _loggerMock;
        private Mock<IGeocodeProvider> _geocodeProviderMock;
        private Mock<IGeocodeJsonProcessor> _geocodeJsonProcessorMock;
        private Mock<ISolarTimeProvider> _solarTimeProviderMock;
        private Mock<ISolarTimeJsonProcessor> _solarTimeJsonProcessorMock;
        private SunsetController _controller;
        private readonly string _geocodeData = "[{}]";
        private readonly string _solarTimeData = "{}";
        private readonly string _cityInput = "city,state,country";
        private readonly DateOnly _dateInput = new DateOnly(2023, 1, 12);
        private readonly string? _tzidInput = "UTC";
        private readonly (float lat, float lon) _geocodes = (It.IsAny<float>(), It.IsAny<float>());
        private readonly (TimeOnly sunrise, TimeOnly sunset) _solarTimes = (It.IsAny<TimeOnly>(), It.IsAny<TimeOnly>());


        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<SunsetController>>();
            _geocodeProviderMock = new Mock<IGeocodeProvider>();
            _geocodeJsonProcessorMock = new Mock<IGeocodeJsonProcessor>();
            _solarTimeProviderMock = new Mock<ISolarTimeProvider>();
            _solarTimeJsonProcessorMock = new Mock<ISolarTimeJsonProcessor>();
            _controller = new SunsetController(_loggerMock.Object, _geocodeProviderMock.Object, _geocodeJsonProcessorMock.Object,
                _solarTimeProviderMock.Object, _solarTimeJsonProcessorMock.Object);
        }

        [Test]
        public async Task GetSunsetTimeReturnsBadRequestIfGeocodeProviderFails()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ThrowsAsync(new Exception());

            var result = await _controller.GetSunsetTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public async Task GetSunsetTimeReturnsBadRequestIfGeocodeJsonIsNotValid()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Throws(new Exception());

            var result = await _controller.GetSunsetTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSunsetTimeReturnsBadRequestIfSolarTimeProviderFails()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_geocodes.lat, _geocodes.lon, _dateInput, _tzidInput)).
                ThrowsAsync(new Exception());

            var result = await _controller.GetSunsetTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSunsetTimeReturnsBadRequestIfSolarTimeJsonIsNotValid()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_geocodes.lat, _geocodes.lon, _dateInput, _tzidInput)).
                ReturnsAsync(_solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessAndCreateSolarTimeInfo(_solarTimeData, _dateInput)).Throws<Exception>();

            var result = await _controller.GetSunsetTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSunsetTimeReturnsOkResultIfSolarTimeJsonIsValid()
        {
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_geocodes.lat, _geocodes.lon, _dateInput, _tzidInput)).
                ReturnsAsync(_solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessAndCreateSolarTimeInfo(_solarTimeData, _dateInput)).Returns(_solarTimes);

            var result = await _controller.GetSunsetTime(_dateInput, _cityInput, _tzidInput);

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                Assert.That(result.Value, Is.InstanceOf<TimeOnly>());
            });
        }
    }
}
