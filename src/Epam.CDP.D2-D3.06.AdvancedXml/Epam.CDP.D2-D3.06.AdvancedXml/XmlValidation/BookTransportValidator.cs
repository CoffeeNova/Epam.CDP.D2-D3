using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Epam.CDP.D2_D3._06.AdvancedXml.XmlValidation
{
    public class BookTransportValidator
    {
        private readonly XmlReaderSettings _settings;

        private const string BookSchemaNamespace = "http://library.by/catalog";
        private const string BooksSchemaLocation = "XmlValidation/BooksSchema.xsd";

        public BookTransportValidator()
        {
            _settings = new XmlReaderSettings();
            _settings.Schemas.Add(BookSchemaNamespace, BooksSchemaLocation);

            _settings.ValidationFlags = _settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            _settings.ValidationType = ValidationType.Schema;
        }

        private void ValidationEventHandler(ref StringBuilder errorBuilder, object sender, ValidationEventArgs e)
        {
            if (sender is XmlReader)
            {
                var errorMsg = $"{e.Message} Line number:{e.Exception.LineNumber}. Line position:{e.Exception.LinePosition}";
                if (errorBuilder.Length > 0)
                    errorBuilder.AppendLine(errorMsg);
                else
                    errorBuilder.Append(errorMsg);
            }
        }

        /// <summary>
        /// Validates XML book transport file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="errors"></param>
        /// <returns><see langword="true"/> if validation successful.</returns>
        public bool ValidateXmlFile(string filePath, out string[] errors)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var xmlName = Path.GetFileName(filePath);
            if (!XmlReader.IsName(xmlName))
                throw new ArgumentException($"Wrong XML file name: {nameof(xmlName)}.");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"XML file with name: {xmlName} was not found.");

            var errorBuilder = new StringBuilder();
            _settings.ValidationEventHandler += (sender, e) => ValidationEventHandler(ref errorBuilder, sender, e);

            using (var reader = XmlReader.Create(filePath, _settings))
            {
                while (reader.Read()) { }
            }

            if (errorBuilder.Length > 0)
            {
                errors = errorBuilder.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                return false;
            }

            errors = new string[0];
            return true;
        }
    }
}
