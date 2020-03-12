using Microsoft.VisualStudio.TestTools.UnitTesting;
using TFS.Client;
using System.IO;

namespace FeatureSync.Tests
{
    [TestClass()]
    public class TestCaseParserTests
    {
        [TestMethod()]
        public void ParseFeaturesTest()
        {
            var client = new TFSClient("https://tfs.server.com/collection", "token");
            foreach (var test in TestCaseParser.ParseFeatures(Directory.GetFiles(@"C:\Source\...", "*.feature")))
                client.UpdateTestCase(test).Wait();
        }
    }
}