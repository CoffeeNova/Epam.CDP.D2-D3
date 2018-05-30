using Epam.CDP.D2_D3._06.AdvancedXml.XmlValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Epam.CDP.D2_D3._06.AdvancedXmlTests.XmlValidation
{
    [TestClass]
    public class BookTransportValidatorTests : AdvancedXmlTestsBase
    {
        private const string XmlValidation = "XmlValidation";

        [TestMethod]
        public void Should_Validate_Successful()
        {
            // arrange
            var validator = new BookTransportValidator();
            var xmlPath = ReturnXmlFullPath("books_valid.xml", XmlValidation);

            // act
            var result = validator.ValidateXmlFile(xmlPath, out var errors);

            // assert
            Assert.IsTrue(result);
            Assert.IsFalse(errors.Any());
        }

        [TestMethod]
        public void Should_Detect_Error_In_ISBN_Element()
        {
            // arrange
            var validator = new BookTransportValidator();
            var xmlPath = ReturnXmlFullPath("books_incorrect_isbn.xml", XmlValidation);

            // act
            var result = validator.ValidateXmlFile(xmlPath, out var errors);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, errors.Length);

        }

        [TestMethod]
        public void Should_Detect_Wrong_Dates()
        {
            // arrange
            var validator = new BookTransportValidator();
            var xmlPath = ReturnXmlFullPath("books_wrong_dates.xml", XmlValidation);

            // act
            var result = validator.ValidateXmlFile(xmlPath, out var errors);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(2, errors.Length);

        }

        [TestMethod]
        public void Should_Detect_Wrong_Genre()
        {
            // arrange
            var validator = new BookTransportValidator();
            var xmlPath = ReturnXmlFullPath("books_wrong_genre.xml", XmlValidation);

            // act
            var result = validator.ValidateXmlFile(xmlPath, out var errors);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(3, errors.Length);

        }
    }
}