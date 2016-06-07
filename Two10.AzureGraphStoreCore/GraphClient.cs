using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace Two10.AzureGraphStoreCore
{
    public class GraphClient
    {
        private readonly CloudTableClient tableClient;

        public GraphClient(CloudStorageAccount account)
        {
            this.tableClient = account.CreateCloudTableClient();
        }

        public Graph GetGraphReference(string name)
        {
            var table = tableClient.GetTableReference(string.Format("wazgraph{0}", name));
            return new Graph(table, name);
        }

        public IEnumerable<Graph> ListGraphs()
        {
            return this.tableClient.ListTablesSegmentedAsync(null).Result.Where(x => x.Name.StartsWith("wazgraph")).Select(x =>
                {
                    var name = x.Name.Replace("wazgraph", "");
                    return this.GetGraphReference(name);
                });
        }

    }
}
