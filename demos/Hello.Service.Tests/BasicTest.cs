using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Hello.Service.Tests
{
    public class BasicTest
    {
        private readonly HelloService _service;

        public BasicTest()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<BasicTest>()
                .Build();

            _service = new ServiceCollection()
                .AddOptions<HelloServiceOptions>().Bind(configuration).ValidateDataAnnotations()
                .Services
                .AddHttpClient<HelloService>()
                .Services
                .BuildServiceProvider()
                .GetRequiredService<HelloService>();
        }

        [Theory]
        [InlineData("John")]
        public async Task SayHello(string name)
        {
            // Act
            var response =  await _service.SayHello(name);
            // Assert
            response.Should().Contain($"Hello, {name}.", because: "The user's name is {0}", becauseArgs: name);
        }
    }
}
