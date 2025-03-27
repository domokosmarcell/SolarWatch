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
        private readonly string _cityInput = "Paks,HU";
        private readonly DateOnly _dateInput = new(2023, 12, 01);
        private readonly string _tzidInput = "UTC";
        private readonly string _geocodeData = @"[
            {
                ""name"": ""Paks"",
                ""local_names"": 
                    {
                        ""ru"": ""Пакш"",
                        ""sr"": ""Пакш"",
                        ""de"": ""Paksch"",
                        ""hu"": ""Paks""
                    },
                ""lat"": 46.6229468,
                ""lon"": 18.8589364,
                ""country"": ""HU""
            }
        ]";
        private readonly City _cityObject = new()
        {
            Name = "Paks",
            Latitude = 46.6229468f,
            Longitude = 18.8589364f,
            State = null,
            Country = "HU"
        };

        private readonly string _solarTimeData = @"{
            ""results"": {
                ""sunrise"": ""2023-12-01T06:06:00+00:00"",
                ""sunset"": ""2023-12-01T15:00:55+00:00"",
                ""solar_noon"": ""2023-12-01T10:33:28+00:00"",
                ""day_length"": 32095,
                ""civil_twilight_begin"": ""2023-12-01T05:33:33+00:00"",
                ""civil_twilight_end"": ""2023-12-01T15:33:22+00:00"",
                ""nautical_twilight_begin"": ""2023-12-01T04:55:45+00:00"",
                ""nautical_twilight_end"": ""2023-12-01T16:11:10+00:00"",
                ""astronomical_twilight_begin"": ""2023-12-01T04:19:22+00:00"",
                ""astronomical_twilight_end"": ""2023-12-01T16:47:33+00:00""
            },
            ""status"": ""OK"",
            ""tzid"": ""UTC""
        }";
        private readonly SolarTimeInfo _solarTimeInfoObject = new()
        {
            City = new City
            {
                Name = "Paks",
                Latitude = 46.6229468f,
                Longitude = 18.8589364f,
                State = null,
                Country = "HU"
            },
            Date = new DateOnly(2023, 12, 01),
            Sunrise = new TimeOnly(6, 6, 0),
            Sunset = new TimeOnly(15, 0, 55),
            Tzid = "UTC"
        };

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
            _cityRepositoryMock.Setup(x => x.GetByName(_cityInput)).ReturnsAsync((City)null);
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_cityObject);
            _cityRepositoryMock.Setup(x => x.Add(_cityObject)).ReturnsAsync(_cityObject);
            _solarTimeInfoRepositoryMock.Setup(x => x.GetByCityDateAndTzid(_cityObject, _dateInput, _tzidInput)).ReturnsAsync((SolarTimeInfo)null);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_cityObject.Latitude, _cityObject.Longitude, _dateInput, _tzidInput)).
                ThrowsAsync(new InvalidOperationException());

            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSunriseTime_ReturnsBadRequest_IfSolarTimeJsonIsNotValid()
        {
            var todayPlusOneYear = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            var invalidDate = DateOnly.FromDateTime(DateTime.Now.AddYears(2));
            var errorMessage = $"The date that you had to provide was invalid!\nGive a date lower than or equal to {todayPlusOneYear} !";

            _cityRepositoryMock.Setup(x => x.GetByName(_cityInput)).ReturnsAsync((City)null);
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_cityObject);
            _cityRepositoryMock.Setup(x => x.Add(_cityObject)).ReturnsAsync(_cityObject);
            _solarTimeInfoRepositoryMock.Setup(x => x.GetByCityDateAndTzid(_cityObject, invalidDate, _tzidInput)).ReturnsAsync((SolarTimeInfo)null);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_cityObject.Latitude, _cityObject.Longitude, invalidDate, _tzidInput)).
                ReturnsAsync(_solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessSolarTimeInfo(_solarTimeData, invalidDate, _cityObject))
                .Throws(new Exception(errorMessage));

            var result = await _controller.GetSunriseTime(invalidDate, _cityInput, _tzidInput);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.That(badRequest.Value, Is.EqualTo(errorMessage));
        }

        [Test]
        public async Task GetSunriseTime_ReturnsOkResult_IfEveryOperationWasSuccessful()
        {
            _cityRepositoryMock.Setup(x => x.GetByName(_cityInput)).ReturnsAsync((City)null);
            _geocodeProviderMock.Setup(x => x.GetGeocode(_cityInput)).ReturnsAsync(_geocodeData);
            _geocodeJsonProcessorMock.Setup(x => x.ProcessGeocodeInfo(_geocodeData, _cityInput)).Returns(_cityObject);
            _cityRepositoryMock.Setup(x => x.Add(_cityObject)).ReturnsAsync(_cityObject);
            _solarTimeInfoRepositoryMock.Setup(x => x.GetByCityDateAndTzid(_cityObject, _dateInput, _tzidInput)).ReturnsAsync((SolarTimeInfo)null);
            _solarTimeProviderMock.Setup(x => x.GetSolarTimes(_cityObject.Latitude, _cityObject.Longitude, _dateInput, _tzidInput)).
                ReturnsAsync(_solarTimeData);
            _solarTimeJsonProcessorMock.Setup(x => x.ProcessSolarTimeInfo(_solarTimeData, _dateInput, _cityObject)).Returns(_solarTimeInfoObject);
            _solarTimeInfoRepositoryMock.Setup(x => x.Add(_solarTimeInfoObject)).ReturnsAsync(_solarTimeInfoObject);

            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                Assert.That(result.Value, Is.EqualTo(_solarTimeInfoObject.Sunrise));
            });
        }
        [Test]
        public async Task GetSunriseTime_ReturnsOkResult_IfCityAndSolarTimeInfoWasFoundInDatabase()
        {
            _cityRepositoryMock.Setup(x => x.GetByName(_cityInput)).ReturnsAsync(_cityObject);
            _solarTimeInfoRepositoryMock.Setup(x => x.GetByCityDateAndTzid(_cityObject, _dateInput, _tzidInput)).ReturnsAsync(_solarTimeInfoObject);
            var result = await _controller.GetSunriseTime(_dateInput, _cityInput, _tzidInput);
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                Assert.That(result.Value, Is.EqualTo(_solarTimeInfoObject.Sunrise));
            });
        }
    }
}
