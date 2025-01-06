using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SolarWatch.Services.ApiProviders;
using SolarWatch.Services.WebClientWrapper;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace SolarWatchTest.SolarTimeTests
{
    [TestFixture]
    public class SolarTimeProviderTest
    {
        private Mock<ILogger<SolarTimeProvider>> _loggerMock;
        private Mock<IWebClient> _webClientMock;
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
            _webClientMock = new Mock<IWebClient>();
            _solarTimeProvider = new SolarTimeProvider(_loggerMock.Object, _webClientMock.Object);
        }

        [Test]
        public void GetSolarTimesThrowsExceptionIfApiCallFails() 
        {
            _webClientMock.Setup(x => x.DownloadString(It.Is<string>((url) => url.Contains(FormattedLat) && url.Contains(FormattedLon) && url.Contains(FormattedDate)))).Throws(new Exception());

            var result = Assert.Throws<Exception>(() => _solarTimeProvider.GetSolarTimes(_randomLatitude, _randomLongitude, _dummyDate, _defaultTzid));

            Assert.That(result, Is.InstanceOf<Exception>());
        }

        [Test]
        public void GetSolarTimesReturnsSolarTimeInfoIfEverythingIsOk()
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
            
            _webClientMock.Setup(x => x.DownloadString(It.Is<string>((url) => url.Contains(FormattedLat) && url.Contains(FormattedLon) && url.Contains(FormattedDate)))).Returns(fakeResponse);

            var result = _solarTimeProvider.GetSolarTimes(_randomLatitude, _randomLongitude, _dummyDate, _defaultTzid);

            Assert.That(result, Is.EqualTo(fakeResponse));
        }
    }
}
