
using System.Text.Json;
using SolarWatch.Models;

namespace SolarWatch.Services.JsonProcessors
{
    public class SolarTimeJsonProcessor : ISolarTimeJsonProcessor
    {
        public SolarTimeInfo ProcessSolarTimeInfo(string solarTimeInfo, DateOnly date, City city)
        {
            JsonDocument json = JsonDocument.Parse(solarTimeInfo);
            var solarTimeInfoElement = json.RootElement;

            ValidateResponseStatus(solarTimeInfoElement);
            ValidateDate(date);
            var resultsElement = solarTimeInfoElement.GetProperty("results");
            var sunrise = GetTimeFromDateTimeOffset(resultsElement.GetProperty("sunrise").GetDateTimeOffset());
            var sunset = GetTimeFromDateTimeOffset(resultsElement.GetProperty("sunset").GetDateTimeOffset());
            var tzid = solarTimeInfoElement.GetProperty("tzid").GetString();

            var newSolarTimeInfo = new SolarTimeInfo()
            {
                City = city,
                Date = date,
                Sunrise = sunrise,
                Sunset = sunset,
                Tzid = tzid
            };
            return newSolarTimeInfo;
        }

        private static TimeOnly GetTimeFromDateTimeOffset(DateTimeOffset time)
        {
            DateTime dateTimeFormat = time.DateTime;
            return TimeOnly.FromDateTime(dateTimeFormat);
        }

        private static void ValidateDate(DateOnly date)
        {
            var todayPlusOneYear = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            if (date > todayPlusOneYear)
                throw new Exception($"The date that you had to provide was invalid!\nGive a date lower than or equal to {todayPlusOneYear} !");
        }
        private static void ValidateResponseStatus(JsonElement solarTimeInfoElement)
        {
            var responseStatusInfo = solarTimeInfoElement.GetProperty("status").GetString();
            if (responseStatusInfo != "OK")
                throw new Exception($"Some problem(s) occurred with the response.\nSunrise/Sunset Api's response status is {responseStatusInfo}.");
        }

    }
}
