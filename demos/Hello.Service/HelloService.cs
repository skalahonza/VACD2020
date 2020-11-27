using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Options;

namespace Hello.Service
{
    public class HelloService
    {
        private readonly HttpClient _client;

        public HelloService(IOptions<HelloServiceOptions> options, HttpClient client)
        {
            _client = client;

            _client.BaseAddress = new Uri(options.Value.BaseUrl);
            _client.DefaultRequestHeaders.Add("x-functions-key", options.Value.ApiKey);
        }

        public async Task<string> SayHello(string name)
        {
            var response = await _client.GetAsync($"api/Function1?name={HttpUtility.UrlEncode(name)}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
