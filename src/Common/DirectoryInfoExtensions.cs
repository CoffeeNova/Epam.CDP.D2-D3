using System;
using System.Collections.Generic;
using System.IO;

namespace Common
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException(nameof(extensions));

            foreach (var ext in extensions)
            {
                foreach (var file in dir.GetFiles(ext))
                    yield return file;
            }
        }
    }
}
