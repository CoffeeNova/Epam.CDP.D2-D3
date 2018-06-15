namespace FileFormatter.Common
{
    public class GlobalSettings : IGlobalSettings
    {
        public bool LoggingEnabled { get; set; }
        public LoggingAspectType LoggingType { get; set; }

        public enum LoggingAspectType
        {
            DynamicProxy,
            CodeRewriting
        }
    }
}
