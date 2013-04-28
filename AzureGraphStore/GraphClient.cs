using Microsoft.WindowsAzure.Storage;

namespace Two10.AzureGraphStore
{
    public class GraphClient
    {
        private readonly CloudStorageAccount account;

        public GraphClient(CloudStorageAccount account)
        {
            this.account = account;
        }

        public Graph GetGraphReference(string name)
        {
            return new Graph(this.account, name);
        }

    }
}
