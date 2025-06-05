namespace VT.Utils.Logger
{
    [System.Flags]
    public enum LogLevel
    {
        None = 0,
        Trace = 1 << 0,     // 1
        Debug = 1 << 1,     // 2
        Warning = 1 << 2,   // 3
        Error = 1 << 3,     // 4
        All = Trace | Debug | Warning | Error // Combination of all
    }

    public interface ILogger
    {
        void Log(LogLevel logLevel, string message, string context, UnityEngine.Color color);
        void LogTrace(string message, string context = "");
        void LogDebug(string message, string context = "");
        void LogWarning(string message, string context = "");
        void LogError(string message, string context = "");
        bool IsEnabled(LogLevel logLevel);
    }
}
