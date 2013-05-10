using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Two10.AzureIndexStore;

namespace Two10.AzureTextSearch
{
    public class TextSearchClient
    {
        private readonly CloudTableClient tableClient;

        public TextSearchClient(CloudStorageAccount account)
        {
            this.tableClient = account.CreateCloudTableClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cacheLifetimeInSeconds">The lifetime of a cached index, set to zero for no caching. Default is 60 seconds.</param>
        /// <returns></returns>
        public TextSearchIndex GetTextSearchIndexReference(string name, int cacheLifetimeInSeconds = 60)
        {
            var table = tableClient.GetTableReference(string.Format("waztextsearch{0}", name));
            if (cacheLifetimeInSeconds <= 0)
            {
                return new TextSearchIndex(new IndexStore<Metadata>(table, name));
            }
            return new TextSearchIndex(new CachedIndexStore<Metadata>(table, name, cacheLifetimeInSeconds));
        }

        public IEnumerable<string> ListTextSearchIndexes()
        {
            return this.tableClient.ListTables().Where(x => x.Name.StartsWith("waztextsearch")).Select(x =>
            {
                var name = x.Name.Replace("waztextsearch", "");
                return name;
            });
        }
    }
}
