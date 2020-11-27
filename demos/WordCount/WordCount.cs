using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace WordCount
{

    public class WordCount : TableEntity
    {
        public const string TABLE_NAME = "wordcount";

        public WordCount()
        {
        }

        public WordCount(string filename, IEnumerable<KeyValuePair<string, int>> words) : this()
        {
            RowKey = filename;
            Words = new Dictionary<string, int>(words);
        }

        public Dictionary<string, int> Words { get; set; }
    }
}
