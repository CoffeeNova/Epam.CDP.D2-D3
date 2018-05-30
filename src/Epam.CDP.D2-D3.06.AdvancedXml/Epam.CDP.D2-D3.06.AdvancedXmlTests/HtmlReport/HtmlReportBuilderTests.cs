using System.IO;
using Epam.CDP.D2_D3._06.AdvancedXml.HtmlReport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epam.CDP.D2_D3._06.AdvancedXmlTests.HtmlReport
{
    [TestClass()]
    public class HtmlReportBuilderTests: AdvancedXmlTestsBase
    {
        private const string HtmlReport = "HtmlReport";

        [TestMethod()]
        public void CreateHtmlReportFileTest()
        {
            // arrange
            var rss = new HtmlReportBuilder();
            var xmlPath = ReturnXmlFullPath("books.xml", HtmlReport);
            var outputPath = ReturnXmlFullPath("", HtmlReport, "Output");

            // act
            rss.CreateRssFeedFile(xmlPath, Path.Combine(AppDirectory, outputPath));
        }
    }
}