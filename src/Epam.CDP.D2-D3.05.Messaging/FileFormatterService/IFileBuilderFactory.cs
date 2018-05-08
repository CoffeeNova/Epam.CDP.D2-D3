namespace FileFormatterService
{
    public interface IFileBuilderFactory
    {
        IFileBuilder GetFileBuilder(FileType fileType, string[] imagesFullNames);
    }
}
