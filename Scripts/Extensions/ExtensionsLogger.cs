// Internal, centralized logger for extension helpers
using UnityEngine;

internal static class ExtensionsLogger
    {
        // Default: unmuted in Editor/Dev, muted in Release
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const bool DefaultMuted = false;
#else
        private const bool DefaultMuted = true;
#endif
        internal static bool Muted { get; private set; } = DefaultMuted;

        private sealed class DefaultLogger : ILogger
        {
            public LogLevel EnabledLevels { get; set; } = LogLevel.All;

            public void Log(LogLevel level, string message, string context, Color color)
            {
                if (ExtensionsLogger.Muted) return;
                if (!IsEnabled(level) || string.IsNullOrEmpty(message)) return;
                string formatted = FormatMessage(level, message, context, color);
                switch (level)
                {
                    case LogLevel.Warning: Debug.LogWarning(formatted); break;
                    case LogLevel.Error: Debug.LogError(formatted); break;
                    default: Debug.Log(formatted); break;
                }
            }

            public void LogTrace(string m, string c = "") => Log(LogLevel.Trace, m, c, new Color(0.70f, 0.85f, 1f));
            public void LogDebug(string m, string c = "") => Log(LogLevel.Debug, m, c, new Color(0.85f, 1f, 0.85f));
            public void LogWarning(string m, string c = "") => Log(LogLevel.Warning, m, c, new Color(1f, 0.9f, 0.55f));
            public void LogError(string m, string c = "") => Log(LogLevel.Error, m, c, new Color(1f, 0.55f, 0.55f));
            public bool IsEnabled(LogLevel lvl) => (EnabledLevels & lvl) != 0;

            private static string FormatMessage(LogLevel lvl, string msg, string ctx, Color col)
            {
                string tag = $"<b><color=#{ColorUtility.ToHtmlStringRGBA(col)}>[{lvl}]</color></b>";
                string c2 = string.IsNullOrEmpty(ctx) ? "" : $" <b><color=#{ColorUtility.ToHtmlStringRGBA(col)}>[{ctx}]</color></b>";
                return $"{tag}{c2}: {msg}";
            }
        }

        internal static readonly ILogger Logger = new DefaultLogger();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Muted = DefaultMuted;
            if (Logger is DefaultLogger d)
                d.EnabledLevels = Muted ? LogLevel.None : LogLevel.All;
        }

        public static void SetMuted(bool muted)
        {
            Muted = muted;
            if (Logger is DefaultLogger d)
                d.EnabledLevels = muted ? LogLevel.None : LogLevel.All;
        }

        public static void SetEnabledLevels(LogLevel levels)
        {
            if (Logger is DefaultLogger d) d.EnabledLevels = levels;
        }
    }

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

    internal interface ILogger
    {
        void Log(LogLevel logLevel, string message, string context, UnityEngine.Color color);
        void LogTrace(string message, string context = "");
        void LogDebug(string message, string context = "");
        void LogWarning(string message, string context = "");
        void LogError(string message, string context = "");
        bool IsEnabled(LogLevel logLevel);
    }