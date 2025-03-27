using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarWatch.Models;
using SolarWatch.Services.JsonProcessors;

namespace SolarWatchTest.GeocodeTests
{
    [TestFixture]
    public class GeocodeJsonProcessorTest
    {
        private GeocodeJsonProcessor _geocodeJsonProcessor;
        [SetUp]
        public void Setup()
        {
            _geocodeJsonProcessor = new GeocodeJsonProcessor();
        }

        [Test]
        public void ProcessGeocodeInfo_ThrowsException_IfApiResponseIsAnErrorObj()
        {
            var cityInput = "London,England,GB";
            var exampleErrorFromApi = @"{
                ""cod"": 401,
                ""message"": ""Invalid API key. Please see https://openweathermap.org/faq#error401 for more info.""
            }";
            var returnedErrorMessage = "Invalid API key. Please see https://openweathermap.org/faq#error401 for more info.";

            var result = Assert.Throws<Exception>( () => _geocodeJsonProcessor.ProcessGeocodeInfo(exampleErrorFromApi, cityInput));

            Assert.That(result.Message, Is.EqualTo(returnedErrorMessage));
        }


        [Test]
        public void ProcessGeocodeInfo_ThrowsException_IfGeocodeInfoIsAnEmptyArray()
        {
            var nonexistentCityInput = "asd";
            var emptyGeocodeInfo = "[]";

            var result = Assert.Throws<Exception>(() => _geocodeJsonProcessor.ProcessGeocodeInfo(emptyGeocodeInfo, nonexistentCityInput));

            Assert.That(result.Message, Is.EqualTo("The city name and country code " +
                "(and where it is needed state name) that you provided is not valid or cannot be found " +
                "in the Geocoding API database!"));
        }

        [Test]
        public void ProcessGeocodeInfo_ThrowsException_IfApiProvidesWrongCity()
        {
            var cityInput = "Ao"; // you want to retrieve a small village in estonia, but you will get a japanese city Yao
            var geocodeInfo = "[{ \"name\": \"Yao\"}]"; // dummy geocodeInfo with only the name bacause it will throw an error(not the same as the input)

            var result = Assert.Throws<Exception>(() => _geocodeJsonProcessor.ProcessGeocodeInfo(geocodeInfo, cityInput));

            Assert.That(result.Message, Is.EqualTo("The city you typed in is not the same as the one that the API provided!\n" +
                    "Type in something else or type the city name and/or country code " +
                    "(and/or the state name) more precisely!"));
        }

        [Test]
        public void ProcessGeocodeInfo_ReturnsCityInfo_IfEverythingIsOk()
        {
            var cityInput = "Paks,HU";
            var geocodeInfo = @"
                [
                    {
                        ""name"": ""Paks"",
                        ""local_names"": {
                            ""de"": ""Paksch"",
                            ""hu"": ""Paks"",
                            ""ru"": ""Пакш"",
                            ""sr"": ""Пакш""
                        },
                        ""lat"": 46.6229468,
                        ""lon"": 18.8589364,
                        ""country"": ""HU""
                    }
                ]
                ";
            var validCity = new City()
            {
                Name = "Paks",
                Latitude = 46.6229468f,
                Longitude = 18.8589364f,
                Country = "HU"
            };

            var result = _geocodeJsonProcessor.ProcessGeocodeInfo(geocodeInfo, cityInput);

            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(validCity.Name));
                Assert.That(result.Latitude, Is.EqualTo(validCity.Latitude));
                Assert.That(result.Longitude, Is.EqualTo(validCity.Longitude));
                Assert.That(result.Country, Is.EqualTo(validCity.Country));
            });
        }
    }
}
