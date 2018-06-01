using System.Collections.Generic;
using System.IO;

namespace Common
{
    public static class StreamExtensions
    {
        public static IEnumerable<byte> StreamToIEnumerable(this Stream stream)
        {
            for (var i = stream.ReadByte(); i != -1; i = stream.ReadByte())
                yield return (byte)i;
        }
    }
}
