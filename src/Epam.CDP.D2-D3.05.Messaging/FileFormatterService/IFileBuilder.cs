using System.IO;

namespace FileFormatterService
{
    public interface IFileBuilder
    {
        void Build(string fileAlias, MemoryStream stream);
        string FileName { get; }
    }
}
