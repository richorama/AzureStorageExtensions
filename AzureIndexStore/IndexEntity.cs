using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Two10.AzureIndexStore
{
    public class IndexEntity<T> : TableEntity
    {

        public IndexEntity()
        {
        }

        public IndexEntity(string key, string value, T meta)
        {
            this.PartitionKey = key;
            this.RowKey = value;
            if (null != meta)
            {
                this.Metadata = JsonConvert.SerializeObject(meta);
            }
        }

        public IndexEntity(IndexEntry<T> entry)
        {
            if (null == entry) throw new ArgumentNullException("entry");

            this.PartitionKey = entry.Key;
            this.RowKey = entry.Value;
            this.Metadata = JsonConvert.SerializeObject(entry.Metadata);
        }

        public string Metadata { get; set; }

        private T GetMetadata()
        {
            if (string.IsNullOrWhiteSpace(this.Metadata)) return default(T);
            return JsonConvert.DeserializeObject<T>(this.Metadata);
        }

        public IndexEntry<T> ToEntry()
        {
            return new IndexEntry<T>(this.PartitionKey, this.RowKey, this.GetMetadata());
        }

    }
}
