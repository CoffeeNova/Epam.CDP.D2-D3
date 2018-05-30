using System.IO;
using Epam.CDP.D2_D3._06.AdvancedXml.RssFeed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epam.CDP.D2_D3._06.AdvancedXmlTests.RssFeed
{
    [TestClass]
    public class RssFeedBuilderTests : AdvancedXmlTestsBase
    {
        private const string RssFeed = "RssFeed";

        [TestMethod]
        public void CreateRssFeedFileTest()
        {
            // arrange
            var rss = new RssFeedBuilder();
            var xmlPath = ReturnXmlFullPath("books.xml", RssFeed);
            var outputPath = ReturnXmlFullPath("", RssFeed, "Output");

            // act
            rss.CreateRssFeedFile(xmlPath, Path.Combine(AppDirectory, outputPath));
        }
    }
}