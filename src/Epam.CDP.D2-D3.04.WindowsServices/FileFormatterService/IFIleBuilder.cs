namespace FileFormatterService
{
    public interface IFileBuilder
    {
        void Build(string fileName);
        string FileExtension { get; }
        string FileName { get; }
    }
}
