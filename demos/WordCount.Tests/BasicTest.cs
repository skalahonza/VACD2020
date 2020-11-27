
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using Xunit;

namespace WordCount.Tests
{
    public class BasicTest
    {
        [Theory]
        [InlineData("Input/empty.txt", "empty.txt", "Output/empty.json")]
        [InlineData("Input/small.txt", "empty.txt", "Output/small.json")]
        public async Task WordCountStream(string input, string name, string expected)
        {
            var result = await Function1.Run(File.OpenRead(input), name, NullLogger.Instance);
            result
                .Words
                .Should()
                .Equal(JsonSerializer.Deserialize<Dictionary<string, int>>(await File.ReadAllTextAsync(expected)));
        }
    }
}
