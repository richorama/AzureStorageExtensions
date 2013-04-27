using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;

namespace Two10.AzureGraphStore.Tests
{
    [TestFixture]
    public class Tests
    {

        [TestFixtureTearDown]
        public void TearDown()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var graphClient = account.CreateCloudGraphClient();
            graphClient.DeleteTables();
        }

        [Test]
        public void TestSingleEntity()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var graphClient = account.CreateCloudGraphClient();
            graphClient.Put(new Triple("Richard", "Loves", "Spam"));

            var triples = graphClient.Get("Richard", "Loves", "Spam").ToArray();
            Assert.AreEqual(1, triples.Length);
            Assert.AreEqual("Richard", triples[0].Subject);
            Assert.AreEqual("Loves", triples[0].Property);
            Assert.AreEqual("Spam", triples[0].Value);
        }

        [Test]
        public void TestMultipleEntities()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var graphClient = account.CreateCloudGraphClient();
            graphClient.Put(new Triple("Bob", "Loves", "DotNet"));
            graphClient.Put("Bob", "Loves", "JavaScript");
            graphClient.Put(new Triple("Bob", "Hates", "Java"));

            var bobLovesTriples = graphClient.Get(subject: "Bob", property: "Loves").ToArray();
            Assert.AreEqual(2, bobLovesTriples.Length);

            var bobHatesTriples = graphClient.Get(subject: "Bob", property: "Hates").ToArray();
            Assert.AreEqual(1, bobHatesTriples.Length);
            Assert.AreEqual("Bob", bobHatesTriples[0].Subject);
            Assert.AreEqual("Hates", bobHatesTriples[0].Property);
            Assert.AreEqual("Java", bobHatesTriples[0].Value);

            var bobHatesJava = graphClient.Get(subject: "Bob", value: "Java").ToArray();
            Assert.AreEqual(1, bobHatesJava.Length);
            Assert.AreEqual("Bob", bobHatesTriples[0].Subject);
            Assert.AreEqual("Hates", bobHatesTriples[0].Property);
            Assert.AreEqual("Java", bobHatesTriples[0].Value);
        }

        [Test]
        public void TestDelete()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var graphClient = account.CreateCloudGraphClient();
            graphClient.Put("Wendy", "Likes", "Cheese");

            var triples = graphClient.Get("Wendy", "Likes", "Cheese").ToArray();
            Assert.AreEqual(1, triples.Length);

            graphClient.Delete(triples[0]);

            triples = graphClient.Get("Wendy", "Likes", "Cheese").ToArray();
            Assert.AreEqual(0, triples.Length);
        }
    }
}
