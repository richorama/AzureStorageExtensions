using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
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

        public TextSearchIndex GetTextSearchIndexReference(string name)
        {
            var table = tableClient.GetTableReference(string.Format("waztextsearch{0}", name));
            return new TextSearchIndex(new CachedIndexStore<Metadata>(table, name, new CacheItemPolicy(){ SlidingExpiration = TimeSpan.FromMinutes(5)}));
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
