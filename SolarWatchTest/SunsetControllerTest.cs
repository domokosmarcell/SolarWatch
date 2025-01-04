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
        public void GetSunsetTimeReturnsBadRequestIfGeocodeProviderFails()
        {
            var dateInput = new DateOnly(2023, 1, 12);
            var cityInput = "city,state,country";
            string? tzidInput = "UTC";
            _geocodeProviderMock.Setup(x => x.GetGeocode(cityInput)).Throws(new Exception());

            var result = _controller.GetSunsetTime(dateInput, cityInput, tzidInput);

            Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public void GetSunsetTimeReturnsBadRequestIfGeocodeJsonIsNotValid()
        {
            var geocodeData = "[{}]";
            var cityInput = "city,state,country";
            var dateInput = new DateOnly(2023, 1, 12);
            string? tzidInput = "UTC";
            _geocodeProviderMock.Setup(x => x.GetGeocode(cityInput)).Returns(geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(geocodeData, cityInput)).Throws(new Exception());

            var result = _controller.GetSunsetTime(dateInput, cityInput, tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void GetSunsetTimeReturnsBadRequestIfSolarTimeProviderFails()
        {
            var geocodeData = "[{}]";
            var cityInput = "city,state,country";
            var dateInput = new DateOnly(2023, 1, 12);
            string? tzidInput = "UTC";
            (float lat, float lon) geocodes = (It.IsAny<float>(), It.IsAny<float>());
            _geocodeProviderMock.Setup(x => x.GetGeocode(cityInput)).Returns(geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(geocodeData, cityInput)).Returns(geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(geocodes.lat, geocodes.lon, dateInput, tzidInput)).
                Throws(new Exception());

            var result = _controller.GetSunsetTime(dateInput, cityInput, tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void GetSunsetTimeReturnsBadRequestIfSolarTimeJsonIsNotValid()
        {
            var geocodeData = "[{}]";
            var solarTimeData = "{}";
            var cityInput = "city,state,country";
            var dateInput = new DateOnly(2023, 1, 12);
            string? tzidInput = "UTC";
            (float lat, float lon) geocodes = (It.IsAny<float>(), It.IsAny<float>());
            _geocodeProviderMock.Setup(x => x.GetGeocode(cityInput)).Returns(geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(geocodeData, cityInput)).Returns(geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(geocodes.lat, geocodes.lon, dateInput, tzidInput)).
                Returns(solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessSolarTimeInfo(solarTimeData, dateInput)).Throws<Exception>();

            var result = _controller.GetSunsetTime(dateInput, cityInput, tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void GetSunsetTimeReturnsOkResultIfSolarTimeJsonIsValid()
        {
            var geocodeData = "[{}]";
            var solarTimeData = "{}";
            var cityInput = "city,state,country";
            var dateInput = new DateOnly(2023, 1, 12);
            string? tzidInput = "UTC";
            (float lat, float lon) geocodes = (It.IsAny<float>(), It.IsAny<float>());
            (TimeOnly sunrise, TimeOnly sunset) solarTimes = (It.IsAny<TimeOnly>(), It.IsAny<TimeOnly>());
            _geocodeProviderMock.Setup(x => x.GetGeocode(cityInput)).Returns(geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(geocodeData, cityInput)).Returns(geocodes);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(geocodes.lat, geocodes.lon, dateInput, tzidInput)).
                Returns(solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessSolarTimeInfo(solarTimeData, dateInput)).Returns(solarTimes);

            var result = _controller.GetSunsetTime(dateInput, cityInput, tzidInput);

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                Assert.That(result.Value, Is.InstanceOf<TimeOnly>());
            });
        }
    }
}
