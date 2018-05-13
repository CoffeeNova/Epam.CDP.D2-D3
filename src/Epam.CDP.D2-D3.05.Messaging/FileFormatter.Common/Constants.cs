using System.Diagnostics.CodeAnalysis;

namespace FileFormatter.Common
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Constants
    {
        public static class FileExtension
        {
            public const string Jpg = "jpg";
            public const string Png = "png";
        }

        public static class ConfigurationConsts
        {
            public const string FileQueueConfigName = "FileQueueConfig";
            public const string ControlQueueName = "ControlQueueConfig";
            public const string StatusQueueTopicName = "StatusQueueConfig";
            public const string FileQueueName = "FileQueueName";
            public const string ControlTopicName = "ControlTopicName";
            public const string StatusTopicName = "StatusTopicName";
            public const string StatusSubscriptionName = "StatusSubscriptionName";
            public const string ConnectionString = "ConnectionString";
        }
    }
}
