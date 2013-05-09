namespace Microsoft.WindowsAzure.Storage
{
    using Two10.AzureJsonStore;

    public static class Extensions
    {
        public static JsonStoreClient CreateCloudJsonClient(this CloudStorageAccount account)
        {
            return new JsonStoreClient(account);
        }
    }
}