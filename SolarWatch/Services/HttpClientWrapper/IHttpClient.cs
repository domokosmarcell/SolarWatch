namespace SolarWatch.Services.HttpClientWrapper
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string url);
    }
}
