namespace Microsoft.WindowsAzure.Storage
{
    using Two10.AzureJsonStore;

    public static class Extensions
    {
        public static JsonClient CreateCloudJsonClient(this CloudStorageAccount account)
        {
            return new JsonClient(account);
        }
    }
}