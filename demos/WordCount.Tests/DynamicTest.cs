
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;

using Moq;

using Xunit;

namespace WordCount.Tests
{
    public class DynamicTest
    {
        private static HttpRequest Mock(string name)
        {
            var mock = new Mock<HttpRequest>();
            mock.SetupGet(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { ["name"] = name }));
            return mock.Object;
        }

        private static IBinder Mock(Stream stream)
        {
            var mock = new Mock<IBinder>();
            mock.Setup(x => x.BindAsync<Stream>(It.IsAny<BlobAttribute>(), It.IsAny<CancellationToken>())).ReturnsAsync(stream);
            return mock.Object;
        }

        [Theory]
        [InlineData("Input/empty.txt", "empty.txt", "Output/empty.json")]
        [InlineData("Input/small.txt", "empty.txt", "Output/small.json")]
        public async Task WordCountDynamic(string input, string name, string expected)
        {
            // Arrange
            var req = Mock(name);
            var binder = Mock(File.OpenRead(input));

            // Act
            var result = await Function2.Run(req, binder, NullLogger.Instance);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok.Value.Should().BeOfType<WordCount>();
            var wc = ok.Value as WordCount;
            wc
                .Words
                .Should()
                .Equal(JsonSerializer.Deserialize<Dictionary<string, int>>(await File.ReadAllTextAsync(expected)));
        }
    }
}
