using System.Collections.Generic;
using Topshelf;

namespace FileFormatterService
{
    public interface IFileFormatterService
    {
        bool Start(HostControl hostControl);
        void Stop();
        ICollection<string> MonitoringPaths { get; set; }
        int NewPageTimeOut { get; set; }
        FileType FileType { get; set; }
        string DamagedPath { get; set; }
        int AttemptCount { get; set; }

        /// <summary>
        /// Max messages size in bytes
        /// </summary>
        int MaxMessagesSize { get; set; }

        string NodeName { get; set; }
    }
}