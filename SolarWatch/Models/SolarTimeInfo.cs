namespace SolarWatch.Models;

public class SolarTimeInfo
{
    public required City City { get; init; }
    public required DateOnly Date {get; init; }
    public required TimeOnly Sunrise { get; init; }
    public required TimeOnly Sunset { get; init; }
    public required string Tzid { get; init; }
}