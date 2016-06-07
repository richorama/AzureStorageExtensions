using Two10.AzureGraphStoreCore;

namespace Microsoft.WindowsAzure.Storage
{
    public static class Extensions
    {
        public static GraphClient CreateCloudGraphClient(this CloudStorageAccount account)
        {
            return new GraphClient(account);
        }

    }
}
