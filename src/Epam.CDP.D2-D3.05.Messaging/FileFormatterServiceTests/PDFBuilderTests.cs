using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FileFormatterService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileFormatterServiceTests
{
    [TestClass()]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PDFBuilderTests
    {
        public TestContext TestContext { get; set; }
        private static string _imageName;
        private FileStream _file;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            _imageName = Path.GetFullPath(Path.Combine(appDir, "..\\..\\", "Image", "img_1.jpg"));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _file = File.Open(_imageName, FileMode.Open);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _file.Close();
            _file.Dispose();
        }

        [TestMethod, ExpectedException(typeof(OutOfMemoryException))]
        public void Should_Throw_Exception_When_File_Locked()
        {
            // Arrange
            var builder = new PDFBuilder(new[] { _imageName });

            // Act / Assert
            using (var stream = new MemoryStream())
            {
                builder.Build("img", stream);
            }
        }
    }
}