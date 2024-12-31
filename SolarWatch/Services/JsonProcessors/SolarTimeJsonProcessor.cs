
using System.Text.Json;

namespace SolarWatch.Services.JsonProcessors
{
    public class SolarTimeJsonProcessor : ISolarTimeJsonProcessor
    {
        public (TimeOnly, TimeOnly) ProcessSolarTimeInfo(string solarTimeInfo)
        {
            JsonDocument json = JsonDocument.Parse(solarTimeInfo);
            var solarTimeInfoElement = json.RootElement;
            var resultsElement = solarTimeInfoElement.GetProperty("results");
            if (solarTimeInfoElement.GetProperty("status").GetString() != "OK")
                throw new Exception("Some problem(s) occured during communicating with the Sunrise/Sunset Api!");

            var sunrise = GetTimeFromDateTimeOffset(resultsElement.GetProperty("sunrise").GetDateTimeOffset());
            var sunset = GetTimeFromDateTimeOffset(resultsElement.GetProperty("sunset").GetDateTimeOffset());

            return (sunrise, sunset);
        }

        private static TimeOnly GetTimeFromDateTimeOffset(DateTimeOffset time)
        {
            DateTime dateTimeFormat = time.DateTime;
            return TimeOnly.FromDateTime(dateTimeFormat);
        }
    }
}
