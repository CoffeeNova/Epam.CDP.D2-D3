namespace FileFormatterService
{
    public interface IFileBuilder
    {
        void Build();
        string FileExtension { get; }
        string FileName { get; set; }
    }
}
