using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Microsoft.WindowsAzure.Storage.Table;

namespace Two10.AzureIndexStore
{
    public class CachedIndexStore<T> : IndexStore<T>
    {
        private MemoryCache cache;
        private int cacheLifetimeInSeconds;

        public CachedIndexStore(CloudTable table, string name, int cacheLifetimeInSeconds = 60)
            : base(table, name)
        {
            this.cache = new MemoryCache(name);
            this.cacheLifetimeInSeconds = cacheLifetimeInSeconds;
        }

        public override IEnumerable<IndexEntry<T>> Query(string key)
        {
            var result = cache.Get(key) as IndexEntry<T>[];
            if (null == result)
            {
                result = base.Query(key).ToArray();
                cache.Add(key, result, new CacheItemPolicy{ AbsoluteExpiration = DateTime.Now.AddSeconds(this.cacheLifetimeInSeconds)});
            }
            return result;
        }

    }
}
