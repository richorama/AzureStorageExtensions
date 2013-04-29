using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Two10.AzureGraphStore
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

    }
}
