using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarWatch.Services.JsonProcessors;
using SolarWatch.Models;

namespace SolarWatchTest.SolarTimeTests
{
    [TestFixture]
    public class SolarTimeJsonProcessorTest
    {
        private SolarTimeJsonProcessor _solarTimeJsonProcessor;
        public static DateOnly MaxValidDate => DateOnly.FromDateTime(DateTime.Now.AddYears(1));
        private City _city;

        [SetUp]
        public void Setup()
        {
            _solarTimeJsonProcessor = new SolarTimeJsonProcessor();
            _city = new City()
            {
                Id = 1,
                Name = "Paks",
                Latitude = 46.6229468f,
                Longitude = 18.8589364f,
                Country = "HU"
            };
        }
        [Test]
        public void ProcessSolarTimeInfoThrowsExceptionIfDateIsBiggerThanTodayPlusOneYear()
        {
            var invalidDate = DateOnly.FromDateTime(DateTime.Now.AddYears(2).AddDays(1));
            var fakeSolarTimeInfo = @"{
                ""results"": {
                    ""sunrise"": ""2023-01-12T07:24:31+01:00"",
                    ""sunset"": ""2023-01-12T16:20:59+01:00"",
                    ""solar_noon"": ""2023-01-12T11:52:45+01:00"",
                    ""day_length"": 32188,
                    ""civil_twilight_begin"": ""2023-01-12T06:52:08+01:00"",
                    ""civil_twilight_end"": ""2023-01-12T16:53:21+01:00"",
                    ""nautical_twilight_begin"": ""2023-01-12T06:14:23+01:00"",
                    ""nautical_twilight_end"": ""2023-01-12T17:31:06+01:00"",
                    ""astronomical_twilight_begin"": ""2023-01-12T05:38:03+01:00"",
                    ""astronomical_twilight_end"": ""2023-01-12T18:07:27+01:00""
                },
                ""status"": ""OK"",
                ""tzid"": ""Europe/Budapest""
            }";

            var result = Assert.Throws<Exception>(() => _solarTimeJsonProcessor.ProcessSolarTimeInfo(fakeSolarTimeInfo, invalidDate, _city));

            Assert.That(result.Message, Is.EqualTo($"The date that you had to provide was invalid!\nGive a date lower than or equal to {MaxValidDate} !"));
        }

        [Test]
        public void ProcessSolarTimeInfoThrowsExceptionIfResponseStatusIsNotOK()
        {
            var randomValidDate = new DateOnly(2023, 01, 12);
            var errorResponseObjectFromApi = @"{
                ""results"": """",
                ""status"": ""UNKNOWN_ERROR""
            }";
            var responseStatus = "UNKNOWN_ERROR";

            var result = Assert.Throws<Exception>(() => _solarTimeJsonProcessor.ProcessSolarTimeInfo(errorResponseObjectFromApi, randomValidDate, _city));

            Assert.That(result.Message, Is.EqualTo($"Some problem(s) occurred with the response.\nSunrise/Sunset Api's response status is {responseStatus}."));
        }
        [Test]
        public void ProcessSolarTimeInfoReturnsSunriseAndSunsetTimesIfEverythingIsOk()
        {
            var randomValidDate = new DateOnly(2023, 01, 12);
            var fakeSolarTimeInfo = @"{
                ""results"": {
                    ""sunrise"": ""2023-01-12T07:24:31+01:00"",
                    ""sunset"": ""2023-01-12T16:20:59+01:00"",
                    ""solar_noon"": ""2023-01-12T11:52:45+01:00"",
                    ""day_length"": 32188,
                    ""civil_twilight_begin"": ""2023-01-12T06:52:08+01:00"",
                    ""civil_twilight_end"": ""2023-01-12T16:53:21+01:00"",
                    ""nautical_twilight_begin"": ""2023-01-12T06:14:23+01:00"",
                    ""nautical_twilight_end"": ""2023-01-12T17:31:06+01:00"",
                    ""astronomical_twilight_begin"": ""2023-01-12T05:38:03+01:00"",
                    ""astronomical_twilight_end"": ""2023-01-12T18:07:27+01:00""
                },
                ""status"": ""OK"",
                ""tzid"": ""Europe/Budapest""
            }";

            var validSolarTimeInfo = new SolarTimeInfo()
            {
                City = _city,
                Date = randomValidDate,
                Sunrise = new TimeOnly(07, 24, 31),
                Sunset = new TimeOnly(16, 20, 59),
                Tzid = "Europe/Budapest"
            };

            var result = _solarTimeJsonProcessor.ProcessSolarTimeInfo(fakeSolarTimeInfo, randomValidDate, _city);

            Assert.Multiple(() =>
            {
                Assert.That(result.City, Is.EqualTo(validSolarTimeInfo.City));
                Assert.That(result.Date, Is.EqualTo(validSolarTimeInfo.Date));
                Assert.That(result.Sunrise, Is.EqualTo(validSolarTimeInfo.Sunrise));
                Assert.That(result.Sunset, Is.EqualTo(validSolarTimeInfo.Sunset));
                Assert.That(result.Tzid, Is.EqualTo(validSolarTimeInfo.Tzid));
            });
        }

    }
}
