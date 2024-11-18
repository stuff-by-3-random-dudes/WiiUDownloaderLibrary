using System.Net.Http;

namespace WiiUDownloaderLibrary.Models.DefaultInjectors
{
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}
