using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SolarWatch.Services.ApiProviders;
using Microsoft.Extensions.Logging;
using System.Globalization;
using SolarWatch.Services.HttpClientWrapper;

namespace SolarWatchTest.SolarTimeTests
{
    [TestFixture]
    public class SolarTimeProviderTest
    {
        private Mock<ILogger<SolarTimeProvider>> _loggerMock;
        private Mock<IHttpClient> _httpClientMock;
        private SolarTimeProvider _solarTimeProvider;
        private static readonly float _randomLatitude = 42.123f;
        private static readonly float _randomLongitude = 16.456f;
        private static readonly DateOnly _dummyDate = new DateOnly(2023, 11, 21);
        private readonly string _defaultTzid = "UTC";
        public static string FormattedLat => _randomLatitude.ToString(CultureInfo.InvariantCulture);
        public static string FormattedLon => _randomLongitude.ToString(CultureInfo.InvariantCulture);
        public static string FormattedDate => _dummyDate.ToString("yyyy-MM-dd");

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SolarTimeProvider>>();
            _httpClientMock = new Mock<IHttpClient>();
            _solarTimeProvider = new SolarTimeProvider(_loggerMock.Object, _httpClientMock.Object);
        }

        [Test]
        public async Task GetSolarTimesThrowsExceptionIfApiCallFails() 
        {
            _httpClientMock.Setup(x => x.GetStringAsync(It.Is<string>((url) => url.Contains(FormattedLat) && url.Contains(FormattedLon) && url.Contains(FormattedDate)))).ThrowsAsync(new Exception());

            var result = Assert.ThrowsAsync<Exception>(async () => await _solarTimeProvider.GetSolarTimes(_randomLatitude, _randomLongitude, _dummyDate, _defaultTzid));

            Assert.That(result, Is.InstanceOf<Exception>());
        }

        [Test]
        public async Task GetSolarTimesReturnsSolarTimeInfoIfEverythingIsOk()
        {
            var fakeResponse = @"{
                ""results"" : ""{
                    ""sunrise"": ""7:06:19 AM"",
                    ""sunset"": ""4:00:47 PM"",
                    ""solar_noon"": ""11:33:33 AM""
                }"",
                ""status"": ""OK"",
                ""tzid"": ""UTC""
            }";
            
            _httpClientMock.Setup(x => x.GetStringAsync(It.Is<string>((url) => url.Contains(FormattedLat) && url.Contains(FormattedLon) && url.Contains(FormattedDate)))).ReturnsAsync(fakeResponse);

            var result = await _solarTimeProvider.GetSolarTimes(_randomLatitude, _randomLongitude, _dummyDate, _defaultTzid);

            Assert.That(result, Is.EqualTo(fakeResponse));
        }
    }
}
