using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Two10.AzureIndexStore;

namespace Two10.AzureTextSearch
{
    public class TextSearchIndex
    {
        private IndexStore<Metadata> indexStore;

        public TextSearchIndex(IndexStore<Metadata> indexStore)
        {
            this.indexStore = indexStore;
        }

        public void Index(string text, string documentId)
        {
            // split the input into tokens
            var tokens = Tokenizer.Tokenize(text);

            // group the token 
            var distinctTokens = tokens.GroupBy(x => x).Select(y => new { Token = y.Key, Count = y.Count() });

            // insert the tokens into the index
            Parallel.ForEach(distinctTokens, x => this.indexStore.Put(x.Token, documentId, new Metadata { Count = x.Count }));
        }

        public IEnumerable<SearchResult> Search(string text)
        {
            // find the unique tokens in the input
            var tokens = Tokenizer.Tokenize(text).Distinct().ToArray();

            if (0 == tokens.Length) return new SearchResult[] { };

            var results = new ConcurrentDictionary<string, IndexEntry<Metadata>[]>();

            Parallel.ForEach(tokens, x => results.TryAdd(x, this.indexStore.Query(x).ToArray()));

            var documents = new ConcurrentDictionary<string, List<string>>();

            // count the results
            Parallel.ForEach(results.Keys, token =>
                {
                    foreach (var doc in results[token])
                    {
                        documents.AddOrUpdate(doc.Value, new List<string>(new[] {token}), (t, x) =>
                            {
                                x.Add(token); 
                                return x;
                            });
                    }
                });

            // resturn the results
            return documents.OrderByDescending(x => x.Value.Count()).Select(x => new SearchResult(x.Key, x.Value.ToArray()));
        }


        public void Delete()
        {
            indexStore.Delete();
        }

        public void DeleteIfExists()
        {
            indexStore.DeleteIfExists();
        }

        public void CreateIfNotExists()
        {
            indexStore.CreateIfNotExists();
        }

        public void Create()
        {
            indexStore.Create();
        }

    }
}
