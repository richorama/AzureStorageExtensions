using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Two10.AzureIndexStore
{
    public class IndexStore<T>
    {
        public string Name { get; private set; }

        private readonly CloudTable table;

        public IndexStore(CloudTable table, string name)
        {
            this.Name = name;
            this.table = table;
        }

        public void Put(string key, string value, T metadata = default(T))
        {
            var entry = new IndexEntry<T>(key, value, metadata);
            this.Put(entry);
        }

        public void Put(IndexEntry<T> entry)
        {
            if (null == entry) throw new ArgumentNullException("entry");

            table.Execute(TableOperation.InsertOrReplace(new IndexEntity<T>(entry)));
        }

        public IndexEntry<T> Get(string key, string value)
        {
            var result = table.Execute(TableOperation.Retrieve<IndexEntity<T>>(key, value));
            if (result.Result as IndexEntity<T> == null) return null;
            return (result.Result as IndexEntity<T>).ToEntry();
        }

        public IEnumerable<IndexEntry<T>> Query(string key)
        {
            var query = new TableQuery<IndexEntity<T>>();
            query.Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", key));
            return table.ExecuteQuery(query).Select(entity => entity.ToEntry());
        }

        public void Delete()
        {
            table.Delete();
        }

        public void DeleteIfExists()
        {
            table.DeleteIfExists();
        }

        public void CreateIfNotExists()
        {
            table.CreateIfNotExists();
        }

        public void Create()
        {
            table.Create();
        }
    }
}
