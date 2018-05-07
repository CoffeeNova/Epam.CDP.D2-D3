namespace FileFormatterService
{
    public interface IFileBuilderFactory
    {
        IFileBuilder GetFileBuilder(FileType fileType, string outputPath, string[] imagesFullNames);
    }
}
