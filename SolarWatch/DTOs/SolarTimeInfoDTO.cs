using SolarWatch.Models;

namespace SolarWatch.DTOs
{
    public class SolarTimeInfoDTO
    {
        public int Id { get; init; }
        public int CityId { get; init; }
        public required string CityName { get; init; }
        public required DateOnly Date { get; init; }
        public required TimeOnly Sunrise { get; init; }
        public required TimeOnly Sunset { get; init; }
        public required string Tzid { get; init; }
    }
}
