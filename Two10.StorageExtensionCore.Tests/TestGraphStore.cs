using System.Linq;
using Xunit;
using Microsoft.WindowsAzure.Storage;
using Two10.AzureGraphStoreCore;

namespace Two10.StorageExtensionCore.Tests
{
    public class TestGraphStore : TestBase
    {


        [Fact]
        public void TearDown()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.DeleteIfExists();
        }

        [Fact]
        public void SetUp()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.CreateIfNotExists();
        }

        [Fact]
        public void TestSingleEntity()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.Put(new Triple("Richard", "Loves", "Spam"));

            var triples = graph.Get("Richard", "Loves", "Spam").ToArray();
            Assert.Equal(1, triples.Length);
            Assert.Equal("Richard", triples[0].Subject);
            Assert.Equal("Loves", triples[0].Property);
            Assert.Equal("Spam", triples[0].Value);
        }

        [Fact]
        public void TestMultipleEntities()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.Put(new Triple("Bob", "Loves", "DotNet"));
            graph.Put("Bob", "Loves", "JavaScript");
            graph.Put(new Triple("Bob", "Hates", "Java"));

            var bobLovesTriples = graph.Get(subject: "Bob", property: "Loves").ToArray();
            Assert.Equal(2, bobLovesTriples.Length);

            var bobHatesTriples = graph.Get(subject: "Bob", property: "Hates").ToArray();
            Assert.Equal(1, bobHatesTriples.Length);
            Assert.Equal("Bob", bobHatesTriples[0].Subject);
            Assert.Equal("Hates", bobHatesTriples[0].Property);
            Assert.Equal("Java", bobHatesTriples[0].Value);

            var bobHatesJava = graph.Get(subject: "Bob", value: "Java").ToArray();
            Assert.Equal(1, bobHatesJava.Length);
            Assert.Equal("Bob", bobHatesTriples[0].Subject);
            Assert.Equal("Hates", bobHatesTriples[0].Property);
            Assert.Equal("Java", bobHatesTriples[0].Value);
        }

        [Fact]
        public void TestDelete()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.Put("Wendy", "Likes", "Cheese");

            var triples = graph.Get("Wendy", "Likes", "Cheese").ToArray();
            Assert.Equal(1, triples.Length);

            graph.Delete(triples[0]);

            triples = graph.Get("Wendy", "Likes", "Cheese").ToArray();
            Assert.Equal(0, triples.Length);
        }

        [Fact]
        public void TestQueryBySingleDimension()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("testsingledimension");
            graph.CreateIfNotExists();
            graph.Put("Dave", "Likes", "Chicken");
            graph.Put("Darren", "Likes", "Chicken");
            graph.Put("Dean", "Hates", "Chicken");
            graph.Put("Dan", "Hates", "Cheese");

            var triples = graph.Get(null, null, "Chicken").ToArray();
            Assert.Equal(3, triples.Length);

            triples = graph.Get("Dave").ToArray();
            Assert.Equal(1, triples.Length);

            triples = graph.Get(null, "Likes").ToArray();
            Assert.Equal(2, triples.Length);

            triples = graph.Get().ToArray();
            Assert.Equal(4, triples.Length);
            graph.Delete();
        }

        [Fact]
        public void TestListGraphs()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var items = graphClient.ListGraphs().Select(x => x.Name).ToArray();
            Assert.Contains("test", items);
            Assert.NotEqual(0, items.Length);
        }


        [Fact]
        public void TestMD5Hash()
        {
            var graphClient = this.Account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test3");
            graph.CreateIfNotExists();

            graph.KeyEncoder = Graph.MD5Hash;
            graph.Put(new Triple("Richard", "Loves", "Spam"));

            var triples = graph.Get("Richard", "Loves", "Spam").ToArray();
            Assert.Equal(1, triples.Length);
            Assert.Equal("Richard", triples[0].Subject);
            Assert.Equal("Loves", triples[0].Property);
            Assert.Equal("Spam", triples[0].Value);
            graph.DeleteIfExists();
        }
    }
}
