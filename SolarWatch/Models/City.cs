namespace SolarWatch.Models;
public class City
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required float Latitude { get; init; }
    public required float Longitude { get; init; }
    public string? State { get; init; }
    public required string Country { get; init; }
}