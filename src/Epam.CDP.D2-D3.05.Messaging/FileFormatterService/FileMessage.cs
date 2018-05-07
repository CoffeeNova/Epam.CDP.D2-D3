using System.IO;
using System.Linq;
using Common;

namespace FileFormatterService
{
    public class FileMessage
    {
        public int Id { get; set; }
        public byte[] Body { get; set; }
        public int Length { get; set; }

        public static FileMessage[] BuildFileMessageSequence(MemoryStream stream, int maxMessagesSize)
        {
            var arr = stream.ToArray();
            var splittedByLength = arr.DivideByLength(maxMessagesSize);
            return splittedByLength.Select((x, i) =>
            {
                var body = x.ToArray();
                return new FileMessage
                {
                    Id = i,
                    Body = body,
                    Length = body.Length
                };
            }).ToArray();
        }
    }

}
