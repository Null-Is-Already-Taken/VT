#if UNITY_EDITOR

using System;
using UnityEngine;

namespace VT.Utils.Logger
{
    /// <summary>
    /// A thread-safe, lazy-initialized Singleton logger implementing ILogger.
    /// </summary>
    public sealed class InternalLogger : ILogger
    {
        // Singleton instance
        private static readonly Lazy<InternalLogger> _instance =
            new(() => new InternalLogger());

        /// <summary>
        /// Gets the singleton instance of the logger.
        /// </summary>
        public static InternalLogger Instance => _instance.Value;

        // Default colors
        private readonly Color defaultTraceColor   = Color.grey;
        private readonly Color defaultDebugColor   = Color.white;
        private readonly Color defaultWarningColor = Color.yellow;
        private readonly Color defaultErrorColor   = Color.red;

        // User-configurable colors
        private Color traceColor;
        private Color debugColor;
        private Color warningColor;
        private Color errorColor;

        // Which levels are enabled
        private LogLevel enabledLogLevels;

        // Private constructor to prevent external instantiation
        private InternalLogger()
        {
            // Initialize defaults
            enabledLogLevels = LogLevel.All;
            ResetColors();
        }

        /// <summary>
        /// Resets colors to their default values.
        /// </summary>
        public void ResetColors()
        {
            traceColor   = defaultTraceColor;
            debugColor   = defaultDebugColor;
            warningColor = defaultWarningColor;
            errorColor   = defaultErrorColor;
        }

        /// <summary>
        /// Enable or disable specific log levels.
        /// </summary>
        public void SetLogLevels(LogLevel logLevels)
        {
            enabledLogLevels = logLevels;
        }

        public bool IsEnabled(LogLevel level)
            => (enabledLogLevels & level) != 0;

        public void Log(LogLevel level, string message, string context, Color color)
        {
            if (!IsEnabled(level)) return;

            string formatted = FormatMessage(level, message, context, color);
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    Debug.Log(formatted);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formatted);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formatted);
                    break;
            }
        }

        public void LogTrace(string message, string context = "")
            => Log(LogLevel.Trace,   message, context, traceColor);

        public void LogDebug(string message, string context = "")
            => Log(LogLevel.Debug,   message, context, debugColor);

        public void LogWarning(string message, string context = "")
            => Log(LogLevel.Warning, message, context, warningColor);

        public void LogError(string message, string context = "")
            => Log(LogLevel.Error,   message, context, errorColor);

        private string FormatMessage(LogLevel level, string message, string context, Color color)
        {
            string lvl = RichTextUtils.FormatText($"[{level}]", color, bold: true);
            string ctx = string.IsNullOrEmpty(context)
                ? string.Empty
                : RichTextUtils.FormatText($"[{context}]", color, bold: true);

            return string.IsNullOrEmpty(ctx)
                ? $"{lvl}: {message}"
                : $"{lvl} {ctx}: {message}";
        }
    }
}

#endif