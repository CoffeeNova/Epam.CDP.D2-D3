using System.Threading.Tasks;

namespace FileFormatterService
{
    public interface IMessagesSender
    {
        Task SendMessagesAsync(FileMessage[] messages);
    }
}
