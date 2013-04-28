using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Two10.AzureGraphStore
{
    public class Graph
    {
        private readonly CloudTable valuePropertyTable;
        private readonly CloudTable subjectValueTable;
        private readonly CloudTable propertySubjectTable;

        public string Name { get; private set; }

        public Graph(CloudStorageAccount account, string name)
        {
            this.Name = name;
            var tableClient = account.CreateCloudTableClient();
            valuePropertyTable = tableClient.GetTableReference(string.Format("wazgraph{0}valueproperty", name));
            subjectValueTable = tableClient.GetTableReference(string.Format("wazgraph{0}subjectvalue", name));
            propertySubjectTable = tableClient.GetTableReference(string.Format("wazgraph{0}propertysubject", name));
        }

        public void Delete()
        {
            valuePropertyTable.Delete();
            propertySubjectTable.Delete();
            subjectValueTable.Delete();
        }

        public void DeleteIfExists()
        {
            valuePropertyTable.DeleteIfExists();
            propertySubjectTable.DeleteIfExists();
            subjectValueTable.DeleteIfExists();
        }

        public void CreateIfNotExists()
        {
            valuePropertyTable.CreateIfNotExists();
            subjectValueTable.CreateIfNotExists();
            propertySubjectTable.CreateIfNotExists();
        }

        public void Create()
        {
            valuePropertyTable.Create();
            subjectValueTable.Create();
            propertySubjectTable.Create();
        }

        public void Put(Triple triple)
        {
            //put in all tables
            var actions = new Action[]
                {
                    () =>
                        {
                            var entity1 = new GraphEntity(triple) { PartitionKey = JoinKey(triple.Property, triple.Subject), RowKey = triple.Value };
                            propertySubjectTable.Execute(TableOperation.InsertOrReplace(entity1));
                        },
                    () =>
                        {
                            var entity2 = new GraphEntity(triple) {PartitionKey = JoinKey(triple.Subject, triple.Value), RowKey = triple.Property};
                            subjectValueTable.Execute(TableOperation.InsertOrReplace(entity2));
                        },
                    () =>
                        {
                            var entity3 = new GraphEntity(triple) {PartitionKey = JoinKey(triple.Value, triple.Property), RowKey = triple.Subject};
                            valuePropertyTable.Execute(TableOperation.InsertOrReplace(entity3));
                        }
                };

            Parallel.Invoke(actions);
        }

        public void Put(string subject, string property, string value)
        {
            this.Put(new Triple(subject, property, value));
        }

        public void Delete(Triple triple)
        {
            var actions = new Action[]
                {
                    () =>
                        {
                            var entity1 = new GraphEntity(triple) {PartitionKey = JoinKey(triple.Property, triple.Subject), RowKey = triple.Value, ETag = "*"};
                            propertySubjectTable.Execute(TableOperation.Delete(entity1));
                        },
                    () =>
                        {
                            var entity2 = new GraphEntity(triple) {PartitionKey = JoinKey(triple.Subject, triple.Value), RowKey = triple.Property, ETag = "*"};
                            subjectValueTable.Execute(TableOperation.Delete(entity2));
                        },
                    () =>
                        {
                            var entity3 = new GraphEntity(triple) {PartitionKey = JoinKey(triple.Value, triple.Property), RowKey = triple.Subject, ETag = "*"};
                            valuePropertyTable.Execute(TableOperation.Delete(entity3));
                        }
                };

            Parallel.Invoke(actions);
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
                    return QueryTriples(propertySubjectTable, property, subject);
                }
                if (hasValue)
                {
                    // subject and value
                    return QueryTriples(subjectValueTable, subject, value);
                }
                return QueryTriples(subjectValueTable, subject);
            }
            if (hasValue)
            {
                if (hasProperty)
                {
                    return QueryTriples(valuePropertyTable, value, property);
                }
                return QueryTriples(valuePropertyTable, value);
            }
            if (hasProperty)
            {
                return QueryTriples(propertySubjectTable, property);
            }
            throw new ArgumentException("Please supply at least one argument");
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

        private IEnumerable<Triple> QueryTriples(CloudTable table, string pk1)
        {
            var query = new TableQuery<GraphEntity>();
            query.Where(string.Format("PartitionKey gt '{0}~' and PartitionKey lt '{0}~~'", pk1));
            return table.ExecuteQuery(query).Select(entity => entity.ToTriple());
        }
    }
}
