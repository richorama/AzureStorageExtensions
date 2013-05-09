namespace Two10.AzureJsonStore
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class JsonClient
    {

        private readonly CloudBlobClient blobClient;

        public JsonClient(CloudStorageAccount account)
        {
            this.blobClient = account.CreateCloudBlobClient();
        }

        public JsonStore<T> GetJsonStoreReference<T>(string name)
        {
            var container = blobClient.GetContainerReference(string.Format("wazjson{0}", name));
            return new JsonStore<T>(container);
        }

        public IEnumerable<string> ListJsonStores()
        {
            return this.blobClient.ListContainers().Where(x => x.Name.StartsWith("wazjson")).Select(x =>
            {
                var name = x.Name.Replace("wazjson", "");
                return name;
            });
        }

    }
}
