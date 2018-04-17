using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace FileFormatterService
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PDFBuilder : IFileBuilder
    {
        private readonly string _outputPath;
        private readonly string[] _imagesPaths;

        public PDFBuilder(string outputPath, string[] imagesFullNames)
        {
            if (string.IsNullOrEmpty(outputPath) || !Directory.Exists(outputPath))
                throw new ArgumentException($"Path '{outputPath}' should exist");

            if (imagesFullNames.Any(string.IsNullOrEmpty) || !imagesFullNames.All(File.Exists))
                throw new ArgumentException("Some images doesn't exists", nameof(imagesFullNames));

            _outputPath = outputPath;
            _imagesPaths = imagesFullNames;
        }

        public void Build()
        {
            if (string.IsNullOrEmpty(FileName))
                throw new InvalidOperationException($"Please set up {FileName} first.");

            var pdfDocRenderer = new PdfDocumentRenderer(true) { Document = CreateDocument() };
            pdfDocRenderer.RenderDocument();
            var path = Path.Combine(_outputPath, BuildFileName());
            pdfDocRenderer.Save(path);
        }

        private string BuildFileName()
        {
            var builder = new StringBuilder(DateTime.Now.ToString("dd-MM-yy hh.mm.ss"));
            builder.Append("_");
            builder.Append(FileName);
            builder.Append(FileExtension);

            return builder.ToString();
        }

        private Document CreateDocument()
        {
            var document = new Document();
            document.AddSection();

            foreach (var imageName in _imagesPaths)
            {
                var para = document.LastSection.AddParagraph();
                para.Format.LeftIndent = "-2cm";
                para.AddImage(imageName);
            }

            return document;
        }

        public string FileExtension => ".pdf";

        public string FileName { get; set; }
    }
}
