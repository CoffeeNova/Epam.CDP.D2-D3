using System;

namespace FileFormatterService
{
    public class FileBuilderFactory : IFileBuilderFactory
    {
        public IFileBuilder GetFileBuilder(FileType fileType, string[] imagesFullNames)
        {
            switch (fileType)
            {
                case FileType.Pdf:
                    return new PDFBuilder(imagesFullNames);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
