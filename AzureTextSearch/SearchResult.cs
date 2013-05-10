
namespace Two10.AzureTextSearch
{
    public class SearchResult
    {
        public SearchResult(string documentId, string[] matchedTokens)
        {
            this.DocumentId = documentId;
            this.MatchedTokens = matchedTokens;
        }

        public string DocumentId { get; set; }

        public string[] MatchedTokens { get; set; }
    }
}
