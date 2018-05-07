using System;

namespace FileFormatterService
{
    public class FileBuilderFactory : IFileBuilderFactory
    {
        public IFileBuilder GetFileBuilder(FileType fileType, string outputPath, string[] imagesFullNames)
        {
            switch (fileType)
            {
                case FileType.Pdf:
                    return new PDFBuilder(outputPath, imagesFullNames);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
