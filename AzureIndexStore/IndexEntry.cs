using System;
namespace Two10.AzureIndexStore
{
    public class IndexEntry<T>
    {
        public IndexEntry(string key, string value, T metadata = default(T))
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

            this.Key = key;
            this.Value = value;
            this.Metadata = metadata;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
        public T Metadata { get; private set; }
    }
}
