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
        private readonly CloudTable table;

        public string Name { get; private set; }

        private const string PROPERTY_SUBJECT = "ps";
        private const string SUBJECT_VALUE = "sv";
        private const string VALUE_PROPERTY = "vp";


        public Graph(CloudTable table, string name)
        {
            this.Name = name;
            this.table = table;
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

        public void Put(Triple triple)
        {
            //put in all tables
            var actions = new Action[]
                {
                    () =>
                        {
                            var entity1 = new GraphEntity(triple) { PartitionKey = JoinKey(PROPERTY_SUBJECT, triple.Property, triple.Subject), RowKey = triple.Value };
                            table.Execute(TableOperation.InsertOrReplace(entity1));
                        },
                    () =>
                        {
                            var entity2 = new GraphEntity(triple) {PartitionKey = JoinKey(SUBJECT_VALUE, triple.Subject, triple.Value), RowKey = triple.Property};
                            table.Execute(TableOperation.InsertOrReplace(entity2));
                        },
                    () =>
                        {
                            var entity3 = new GraphEntity(triple) {PartitionKey = JoinKey(VALUE_PROPERTY, triple.Value, triple.Property), RowKey = triple.Subject};
                            table.Execute(TableOperation.InsertOrReplace(entity3));
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
                            var entity1 = new GraphEntity(triple) {PartitionKey = JoinKey(PROPERTY_SUBJECT, triple.Property, triple.Subject), RowKey = triple.Value, ETag = "*"};
                            table.Execute(TableOperation.Delete(entity1));
                        },
                    () =>
                        {
                            var entity2 = new GraphEntity(triple) {PartitionKey = JoinKey(SUBJECT_VALUE, triple.Subject, triple.Value), RowKey = triple.Property, ETag = "*"};
                            table.Execute(TableOperation.Delete(entity2));
                        },
                    () =>
                        {
                            var entity3 = new GraphEntity(triple) {PartitionKey = JoinKey(VALUE_PROPERTY, triple.Value, triple.Property), RowKey = triple.Subject, ETag = "*"};
                            table.Execute(TableOperation.Delete(entity3));
                        }
                };

            Parallel.Invoke(actions);
        }

        public void Delete(string subject, string property, string value)
        {
            this.Delete(new Triple(subject, property, value));
        }

        private static string JoinKey(string dimension, string value1, string value2)
        {
            return string.Format("{0}~{1}~{2}", dimension, value1, value2);
        }

        public IEnumerable<Triple> Get(string subject = "", string property = "", string value = "")
        {
            var hasSubject = !string.IsNullOrWhiteSpace(subject);
            var hasProperty = !string.IsNullOrWhiteSpace(property);
            var hasValue = !string.IsNullOrWhiteSpace(value);

            // this is where we wish .NET has pattern matching
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
                    return QueryTriples(table, PROPERTY_SUBJECT, property, subject);
                }
                if (hasValue)
                {
                    // subject and value
                    return QueryTriples(table, SUBJECT_VALUE, subject, value);
                }
                return QueryTriples(table, SUBJECT_VALUE, subject);
            }
            if (hasValue)
            {
                if (hasProperty)
                {
                    return QueryTriples(table, VALUE_PROPERTY, value, property);
                }
                return QueryTriples(table, VALUE_PROPERTY, value);
            }
            if (hasProperty)
            {
                return QueryTriples(table, PROPERTY_SUBJECT, property);
            }
            throw new ArgumentException("Please supply at least one argument");
        }

        private IEnumerable<Triple> RetrieveSingleTriple(string subject, string property, string value)
        {
            var val = table.Execute(TableOperation.Retrieve<GraphEntity>(JoinKey(SUBJECT_VALUE,subject, value), property));
            if (null != val.Result)
            {
                yield return new Triple(subject, property, value);
            }
        }

        private IEnumerable<Triple> QueryTriples(CloudTable table, string dimension, string pk1, string pk2)
        {
            var query = new TableQuery<GraphEntity>();
            query.Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", JoinKey(dimension, pk1, pk2)));
            return table.ExecuteQuery(query).Select(entity => entity.ToTriple());
        }

        private IEnumerable<Triple> QueryTriples(CloudTable table, string dimension, string pk1)
        {
            var query = new TableQuery<GraphEntity>();
            query.Where(string.Format("PartitionKey gt '{1}~{0}~' and PartitionKey lt '{1}~{0}~~'", pk1, dimension));
            return table.ExecuteQuery(query).Select(entity => entity.ToTriple());
        }
    }
}
