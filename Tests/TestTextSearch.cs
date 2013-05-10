using System.Linq;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;
using Two10.AzureTextSearch;

namespace Two10.StorageExtension.Tests
{
    [TestFixture]
    public class TestTextSearch
    {
        [Test]
        public void TestTokenizer()
        {
            var tokens = Tokenizer.Tokenize("the cat sat on the mat");
            Assert.AreEqual(new[] { "cat", "sat", "mat" }, tokens);

            tokens = Tokenizer.Tokenize("the cat sat on the mat!");
            Assert.AreEqual(new[] { "cat", "sat", "mat" }, tokens);

            tokens = Tokenizer.Tokenize("The cat SAT ON the mat!");
            Assert.AreEqual(new[] { "cat", "sat", "mat" }, tokens);
        }

        private CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;

        [TestFixtureTearDown]
        public void TearDown()
        {
            var textClient = account.CreateCloudTextSearchClient();
            var text = textClient.GetTextSearchIndexReference("test");
            text.DeleteIfExists();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var textClient = account.CreateCloudTextSearchClient();
            var text = textClient.GetTextSearchIndexReference("test");
            text.CreateIfNotExists();
        }

        [Test]
        public void TestIndex()
        {
            var textClient = account.CreateCloudTextSearchClient();
            var text = textClient.GetTextSearchIndexReference("test");
            text.Index(LOREM, "1");
            var documents = text.Search("consectetur incididunt veniam").ToArray();
            Assert.AreEqual(1, documents.Length);
            Assert.AreEqual("1", documents[0].DocumentId);
        }

        [Test]
        public void TestIndex2()
        {
            var textClient = account.CreateCloudTextSearchClient();
            var text = textClient.GetTextSearchIndexReference("test2");
            text.CreateIfNotExists();

            text.Index(LOREM, "1");
            text.Index(LOREM + " Richard", "2");
            var documents = text.Search("Lorem").ToArray();
            Assert.AreEqual(2, documents.Length);

            documents = text.Search("Richard").ToArray();
            Assert.AreEqual(1, documents.Length);
            Assert.AreEqual("2", documents[0].DocumentId);

            documents = text.Search("richard lorem").ToArray();
            Assert.AreEqual(2, documents.Length);
            Assert.AreEqual("2", documents[0].DocumentId);
            Assert.AreEqual("1", documents[1].DocumentId);

            Assert.AreEqual(2, documents[0].MatchedTokens.Length);
            Assert.AreEqual(1, documents[1].MatchedTokens.Length);
            Assert.AreEqual("lorem", documents[1].MatchedTokens[0]);
        }

        private const string LOREM = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

    }
}
