using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WordCount
{
    public static class Function2
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] 
            HttpRequest req,
            IBinder binder,
            ILogger log)
        {
            string filename = req.Query["name"];
            var input = await binder.BindAsync<Stream>(new BlobAttribute($"lyrics/{filename}", FileAccess.Read) { Connection = "Blob" });

            log.LogInformation($"C# HTTP trigger function Processed blob\n Name:{filename} \n Size: {input.Length} Bytes");
            var words = new Dictionary<string, int>();
            using var reader = new StreamReader(input);
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                foreach (var item in line.Split())
                {
                    var word = new string(item.Where(c => char.IsLetterOrDigit(c)).ToArray());
                    if (string.IsNullOrEmpty(word)) continue;
                    if (words.ContainsKey(word))
                        words[word]++;
                    else words[word] = 1;
                }
            }
            return new OkObjectResult(new WordCount(filename, words.OrderByDescending(x => x.Value).Take(20)));
        }
    }
}
