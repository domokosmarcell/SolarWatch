
using System.Text.Json;
using SolarWatch.Models;

namespace SolarWatch.Services.JsonProcessors
{
    public class GeocodeJsonProcessor : IGeocodeJsonProcessor
    {
        public City ProcessGeocodeInfo(string geocodeInfo, string city)
        {
            JsonDocument json = JsonDocument.Parse(geocodeInfo);
            JsonElement jsonRootElement = json.RootElement;
            
            CheckIfApiResponseIsAnErrorObject(jsonRootElement);
            ValidateTheExistenceOfCities(jsonRootElement);
            ValidateCityNameCorrectnessInResponse(jsonRootElement, city);

            var geocodeInfoElement = jsonRootElement[0];
            var name = geocodeInfoElement.GetProperty("name").GetString();
            float lat = geocodeInfoElement.GetProperty("lat").GetSingle();
            float lon = geocodeInfoElement.GetProperty("lon").GetSingle();
            var country = geocodeInfoElement.GetProperty("country").GetString();

            bool isStateExists = geocodeInfoElement.TryGetProperty("state", out JsonElement stateElement);
            string? state = null;
            if (isStateExists) state = stateElement.GetString();

            var newCityObject = new City()
            {
                Name = name,
                Latitude = lat,
                Longitude = lon,
                State = state,
                Country = country
            };
            return newCityObject;
        }

        private static void CheckIfApiResponseIsAnErrorObject(JsonElement jsonRootElement)
        {
            if(jsonRootElement.ValueKind != JsonValueKind.Array)
            {
                var errorMessage = jsonRootElement.GetProperty("message").GetString();
                throw new Exception(errorMessage);
            }
        }



        private static void ValidateTheExistenceOfCities(JsonElement jsonRootElement)
        {
            if (jsonRootElement.GetArrayLength() == 0)
            {
                throw new Exception("The city name and country code " +
                    "(and where it is needed state name) that you provided is not valid or cannot be found " +
                    "in the Geocoding API database!");
            }
        }

        private static void ValidateCityNameCorrectnessInResponse(JsonElement jsonRootElement, string city)
        {
            if (!jsonRootElement[0].GetProperty("name").GetString().Equals(city.Split(",")[0], StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("The city you typed in is not the same as the one that the API provided!\n" +
                    "Type in something else or type the city name and/or country code " +
                    "(and/or the state name) more precisely!");
            }
        }
    }
}
