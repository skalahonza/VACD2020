using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;

using Moq;

using Xunit;

namespace HelloFunctionApp.Tests
{
    public class BasicTest
    {
        private static HttpRequest Mock(Stream body)
        {
            var mock = new Mock<HttpRequest>();
            mock.SetupGet(x => x.Body).Returns(body);
            mock.SetupGet(x => x.Query).Returns(new QueryCollection());
            return mock.Object;
        }

        private static HttpRequest Mock(string name)
        {
            var mock = new Mock<HttpRequest>();
            mock.SetupGet(x => x.Body).Returns(new MemoryStream()); // empty stream
            mock.SetupGet(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { ["name"] = name }));
            return mock.Object;
        }

        private static void AssertMessage(IActionResult result, string name)
        {
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok.Value.Should().BeOfType<string>();
            var message = ok.Value as string;
            message.Should().Contain($"Hello, {name}.", because: "The user's name is {0}", becauseArgs: name);
        }

        [Theory]
        [InlineData("John")]
        public async Task SayHelloQuery(string name)
        {
            // Arrange
            var req = Mock(name);

            // Act
            var result = await Function1.Run(req, NullLogger.Instance);

            // Assert
            AssertMessage(result, name);
        }

        [Theory]
        [InlineData("Input/john.json", "John")]
        public async Task SayHelloBody(string input, string name)
        {
            // Arrange
            var req = Mock(File.OpenRead(input));

            // Act
            var result = await Function1.Run(req, NullLogger.Instance);

            // Assert
            AssertMessage(result, name);
        }
    }
}
