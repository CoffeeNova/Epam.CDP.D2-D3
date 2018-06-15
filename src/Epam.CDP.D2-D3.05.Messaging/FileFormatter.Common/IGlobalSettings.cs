namespace FileFormatter.Common
{
    public interface IGlobalSettings
    {
        bool LoggingEnabled { get; set; }
        GlobalSettings.LoggingAspectType LoggingType { get; set; }
    }
}