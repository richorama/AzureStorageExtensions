namespace Microsoft.WindowsAzure.Storage
{
    using Two10.AzureTextSearch;

    public static class Extensions
    {
        public static TextSearchClient CreateCloudTextSearchClient(this CloudStorageAccount account)
        {
            return new TextSearchClient(account);
        }
    }
}