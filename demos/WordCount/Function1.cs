using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace WordCount
{
    public static class Function1
    {
        [FunctionName(nameof(Function1))]
        [return: Table(WordCount.TABLE_NAME, "Function1")]
        public static async Task<WordCount> Run([BlobTrigger("lyrics/{filename}", Connection = "Blob")]Stream input, string filename, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{filename} \n Size: {input.Length} Bytes");
            var words = new Dictionary<string, int>();
            using var reader = new StreamReader(input);
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                foreach (var item in line.Split())
                {
                    var word = new string (item.Where(c => char.IsLetterOrDigit(c)).ToArray());
                    if (string.IsNullOrEmpty(word)) continue;
                    if (words.ContainsKey(word))
                        words[word]++;
                    else words[word] = 1;
                }
            }
            return new WordCount(filename, words.OrderByDescending(x => x.Value).Take(20));
        }
    }
}
