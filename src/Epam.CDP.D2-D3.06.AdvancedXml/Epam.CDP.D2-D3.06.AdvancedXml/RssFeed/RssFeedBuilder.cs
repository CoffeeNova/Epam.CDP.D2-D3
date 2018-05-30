using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Epam.CDP.D2_D3._06.AdvancedXml.RssFeed
{
    public class RssFeedBuilder
    {
        private const string RssFeedXsltLocation = "RssFeed/RssFeed.xslt";

        public void CreateRssFeedFile(string xmlPath, string outputPath, string outputFileName = null)
        {
            if (string.IsNullOrEmpty(xmlPath))
                throw new ArgumentNullException(nameof(xmlPath));

            var xmlName = Path.GetFileName(xmlPath);
            if (!XmlReader.IsName(xmlName))
                throw new ArgumentException($"Wrong XML file name: {nameof(xmlName)}.");

            if (!File.Exists(xmlPath))
                throw new FileNotFoundException($"XML file with name: {xmlName} was not found.");

            var xsl = new XslCompiledTransform();
            xsl.Load(RssFeedXsltLocation);
            var xslParams = new XsltArgumentList();
            xslParams.AddParam("Date", "", DateTime.Now.ToLongDateString());

            var outputFullPath = outputFileName == null
                ? Path.Combine(outputPath, Path.GetFileNameWithoutExtension(xmlName) + "_" + DateTime.Now.ToString("dd-MM-yy_hh-mm-ss") + ".xml")
                : Path.Combine(outputPath, outputFileName);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            using (var writer = XmlWriter.Create(outputFullPath, new XmlWriterSettings { WriteEndDocumentOnClose = true }))
            {
                xsl.Transform(xmlPath, xslParams, writer);
            }
        }
    }
}
