using System.Linq;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;

namespace Two10.AzureGraphStore.Tests
{
    [TestFixture]
    public class Tests
    {

        private CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;

        [TestFixtureTearDown]
        public void TearDown()
        {
            var graphClient = account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.DeleteIfExists();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var graphClient = account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.CreateIfNotExists();
        }

        [Test]
        public void TestSingleEntity()
        {
            var graphClient = account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.Put(new Triple("Richard", "Loves", "Spam"));

            var triples = graph.Get("Richard", "Loves", "Spam").ToArray();
            Assert.AreEqual(1, triples.Length);
            Assert.AreEqual("Richard", triples[0].Subject);
            Assert.AreEqual("Loves", triples[0].Property);
            Assert.AreEqual("Spam", triples[0].Value);
        }

        [Test]
        public void TestMultipleEntities()
        {
            var graphClient = account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.Put(new Triple("Bob", "Loves", "DotNet"));
            graph.Put("Bob", "Loves", "JavaScript");
            graph.Put(new Triple("Bob", "Hates", "Java"));

            var bobLovesTriples = graph.Get(subject: "Bob", property: "Loves").ToArray();
            Assert.AreEqual(2, bobLovesTriples.Length);

            var bobHatesTriples = graph.Get(subject: "Bob", property: "Hates").ToArray();
            Assert.AreEqual(1, bobHatesTriples.Length);
            Assert.AreEqual("Bob", bobHatesTriples[0].Subject);
            Assert.AreEqual("Hates", bobHatesTriples[0].Property);
            Assert.AreEqual("Java", bobHatesTriples[0].Value);

            var bobHatesJava = graph.Get(subject: "Bob", value: "Java").ToArray();
            Assert.AreEqual(1, bobHatesJava.Length);
            Assert.AreEqual("Bob", bobHatesTriples[0].Subject);
            Assert.AreEqual("Hates", bobHatesTriples[0].Property);
            Assert.AreEqual("Java", bobHatesTriples[0].Value);
        }

        [Test]
        public void TestDelete()
        {
            var graphClient = account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("test");
            graph.Put("Wendy", "Likes", "Cheese");

            var triples = graph.Get("Wendy", "Likes", "Cheese").ToArray();
            Assert.AreEqual(1, triples.Length);

            graph.Delete(triples[0]);

            triples = graph.Get("Wendy", "Likes", "Cheese").ToArray();
            Assert.AreEqual(0, triples.Length);
        }

        [Test]
        public void TestQueryBySingleDimension()
        {
            var graphClient = account.CreateCloudGraphClient();
            var graph = graphClient.GetGraphReference("testsingledimension");
            graph.CreateIfNotExists();
            graph.Put("Dave", "Likes", "Chicken");
            graph.Put("Darren", "Likes", "Chicken");
            graph.Put("Dean", "Hates", "Chicken");
            graph.Put("Dan", "Hates", "Cheese");

            var triples = graph.Get(null, null, "Chicken").ToArray();
            Assert.AreEqual(3, triples.Length);

            triples = graph.Get("Dave").ToArray();
            Assert.AreEqual(1, triples.Length);

            triples = graph.Get(null, "Likes").ToArray();
            Assert.AreEqual(2, triples.Length);

            triples = graph.Get().ToArray();
            Assert.AreEqual(4, triples.Length);
            graph.Delete();
        }

        [Test]
        public void TestListGraphs()
        {
            var graphClient = account.CreateCloudGraphClient();
            var items = graphClient.ListGraphs().Select(x => x.Name).ToArray();
            Assert.Contains("test", items);
            Assert.AreEqual(1, items.Length);
        }

    }
}
