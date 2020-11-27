using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

namespace Hello.Service
{
    public class HelloServiceOptions : IOptions<HelloServiceOptions>
    {
        public HelloServiceOptions Value => this;

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string BaseUrl { get; set; }
    }
}
