using Two10.AzureIndexStore;

namespace Microsoft.WindowsAzure.Storage
{
    public static class Extensions
    {
        public static IndexClient CreateCloudIndexClient(this CloudStorageAccount account)
        {
            return new IndexClient(account);
        }

    }
}
