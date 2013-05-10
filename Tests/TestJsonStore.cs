using System.Linq;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;

namespace Two10.StorageExtension.Tests
{
    internal class Foo
    {
        public string Bar { get; set; }
        public int Baz { get; set; }
    }

    [TestFixture]
    public class Tests
    {

        private CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;

        [TestFixtureTearDown]
        public void TearDown()
        {
            var jsonClient = account.CreateCloudJsonClient();
            var store = jsonClient.GetJsonStoreReference<Foo>("testfoo");
            store.DeleteIfExists();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var jsonClient = account.CreateCloudJsonClient();
            var store = jsonClient.GetJsonStoreReference<Foo>("testfoo");
            store.CreateIfNotExists();
        }

        [Test]
        public void TestSingleEntity()
        {
            var jsonClient = account.CreateCloudJsonClient();
            var store = jsonClient.GetJsonStoreReference<Foo>("testfoo");

            var foo1 = new Foo { Bar = "BAR", Baz = 3 };
            store.Put("item1", foo1);

            var foo2 = store.Get("item1");
            Assert.IsNotNull(foo2);
            Assert.AreEqual(3, foo2.Baz);
            Assert.AreEqual("BAR", foo2.Bar);

            var foo3 = store.Get("itemdoesnotexist");
            Assert.IsNull(foo3);

            var items = store.Query().ToArray();
            Assert.AreEqual(1, items.Length);
            Assert.AreEqual("item1", items[0]);

            store.Delete("item1");
            items = store.Query().ToArray();
            Assert.AreEqual(0, items.Length);
        }

        [Test]
        public void TestQueryTables()
        {
            var jsonClient = account.CreateCloudJsonClient();
            var stores = jsonClient.ListJsonStores().ToArray();
            Assert.AreEqual(1, stores.Length);
            Assert.AreEqual("testfoo", stores[0]);
        }


    }
}
