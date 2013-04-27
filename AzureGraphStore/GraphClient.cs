using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Two10.AzureGraphStore
{
    public class GraphClient
    {
        private readonly CloudTable valuePropertyTable;
        private readonly CloudTable subjectValueTable;
        private readonly CloudTable subjectPropertyTable;

        public GraphClient(CloudStorageAccount account)
        {
            var tableClient = account.CreateCloudTableClient();
            valuePropertyTable = tableClient.GetTableReference("wazgraphvalueproperty");
            subjectValueTable = tableClient.GetTableReference("wazgraphsubjectvalue");
            subjectPropertyTable = tableClient.GetTableReference("wazgraphsubjectproperty");
            valuePropertyTable.CreateIfNotExists();
            subjectValueTable.CreateIfNotExists();
            subjectPropertyTable.CreateIfNotExists();
        }

        public void DeleteTables()
        {
            valuePropertyTable.Delete();
            subjectPropertyTable.Delete();
            subjectValueTable.Delete();
        }

        public void Put(Triple triple)
        {
            //put in all tables
            var actions = new List<Action>(3);

            actions.Add(() =>
                {
                    var entity1 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Subject, triple.Property), RowKey = triple.Value };
                    subjectPropertyTable.Execute(TableOperation.InsertOrReplace(entity1));
                });

            actions.Add(() =>
                {
                    var entity2 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Subject, triple.Value), RowKey = triple.Property };
                    subjectValueTable.Execute(TableOperation.InsertOrReplace(entity2));
                });

            actions.Add(() =>
                {
                    var entity3 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Value, triple.Property), RowKey = triple.Subject };
                    valuePropertyTable.Execute(TableOperation.InsertOrReplace(entity3));
                });
            Parallel.Invoke(actions.ToArray());
        }

        public void Put(string subject, string property, string value)
        {
            this.Put(new Triple(subject, property, value));
        }

        public void Delete(Triple triple)
        {
            var actions = new List<Action>(3);

            actions.Add(() =>
            {
                var entity1 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Subject, triple.Property), RowKey = triple.Value, ETag = "*" };
                subjectPropertyTable.Execute(TableOperation.Delete(entity1));
            });

            actions.Add(() =>
            {
                var entity2 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Subject, triple.Value), RowKey = triple.Property, ETag = "*" };
                subjectValueTable.Execute(TableOperation.Delete(entity2));
            });

            actions.Add(() =>
            {
                var entity3 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Value, triple.Property), RowKey = triple.Subject, ETag = "*" };
                valuePropertyTable.Execute(TableOperation.Delete(entity3));
            });
            Parallel.Invoke(actions.ToArray());
        }

        public void Delete(string subject, string property, string value)
        {
            this.Delete(new Triple(subject, property, value));
        }

        private static string JoinKey(string value1, string value2)
        {
            return string.Format("{0}~{1}", value1, value2);
        }

        public IEnumerable<Triple> Get(string subject = "", string property = "", string value = "")
        {
            var hasSubject = !string.IsNullOrWhiteSpace(subject);
            var hasProperty = !string.IsNullOrWhiteSpace(property);
            var hasValue = !string.IsNullOrWhiteSpace(value);

            // this is where we wished .NET had pattern matching
            if (hasSubject && hasProperty && hasValue)
            {
                // simple case, retrieve the entity
                return RetrieveSingleTriple(subject, property, value);
            }
            if (hasSubject)
            {
                // find by subject
                if (hasProperty)
                {
                    // subject and property
                    return QueryTriples(subjectPropertyTable, subject, property);
                }
                if (hasValue)
                {
                    // subject and value
                    return QueryTriples(subjectValueTable, subject, value);
                }
                throw new ArgumentException("You must supply at least two arguments");
            }
            if (hasValue)
            {
                if (hasProperty)
                {
                    return QueryTriples(valuePropertyTable, value, property);
                }
                throw new ArgumentException("you must supply at least two arguments");
            }
            throw new ArgumentException("you must supply at least two arguments");
        }

        private IEnumerable<Triple> RetrieveSingleTriple(string subject, string property, string value)
        {
            var val = subjectValueTable.Execute(TableOperation.Retrieve<GraphEntity>(JoinKey(subject, value), property));
            if (null != val.Result)
            {
                yield return new Triple(subject, property, value);
            }
        }

        private IEnumerable<Triple> QueryTriples(CloudTable table, string pk1, string pk2)
        {
            var query = new TableQuery<GraphEntity>();
            query.Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", JoinKey(pk1, pk2)));
            return table.ExecuteQuery(query).Select(entity => entity.ToTriple());
        }
    }
}
