namespace SolarWatch.Models;

public class SolarTimeInfo
{
    public int CityId { get; init; }
    public int Id { get; init; }
    public required City City { get; init; }
    public required DateOnly Date {get; init; }
    public TimeOnly? Sunrise { get; init; }
    public TimeOnly? Sunset { get; init; }
    public required string Tzid { get; init; }
}