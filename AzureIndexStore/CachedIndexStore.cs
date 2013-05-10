using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Microsoft.WindowsAzure.Storage.Table;

namespace Two10.AzureIndexStore
{
    public class CachedIndexStore<T> : IndexStore<T>
    {
        private MemoryCache cache;
        private CacheItemPolicy policy;

        public CachedIndexStore(CloudTable table, string name, CacheItemPolicy policy)
            : base(table, name)
        {
            this.cache = new MemoryCache(name);
            this.policy = policy;

        }

        public override IEnumerable<IndexEntry<T>> Query(string key)
        {
            var result = cache.Get(key) as IndexEntry<T>[];
            if (null == result)
            {
                result = base.Query(key).ToArray();
                cache.Add(key, result, policy);
            }
            return result;
        }

    }
}
