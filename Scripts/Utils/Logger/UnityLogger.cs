using UnityEngine;
using System;
using VT.Patterns.SingletonPattern;
using Sirenix.OdinInspector;

namespace VT.Utils.Logger
{
    public class UnityLogger : Singleton<UnityLogger>, ILogger
    {
        private readonly Color defaultTraceColor = Color.grey;
        private readonly Color defaultDebugColor = Color.white;
        private readonly Color defaultWarningColor = Color.yellow;
        private readonly Color defaultErrorColor = Color.red;

        [SerializeField] private LogLevel enabledLogLevels;

        [FoldoutGroup("Color Settings"), SerializeField] private Color traceColor = Color.grey;
        [FoldoutGroup("Color Settings"), SerializeField] private Color debugColor = Color.white;
        [FoldoutGroup("Color Settings"), SerializeField] private Color warningColor = Color.yellow;
        [FoldoutGroup("Color Settings"), SerializeField] private Color errorColor = Color.red;

        protected override void Awake()
        {
            base.Awake();

            SetLogLevels(enabledLogLevels);
        }

        [Button]
        private void Reset()
        {
            traceColor = defaultTraceColor;
            debugColor = defaultDebugColor;
            warningColor = defaultWarningColor;
            errorColor = defaultErrorColor;
        }

        public void SetLogLevels(LogLevel logLevel)
        {
            enabledLogLevels = logLevel;
        }

        public void Log(LogLevel level, string message, string context, Color color)
        {
            string formattedMessage = FormatMessage(level, message, context, color);

            switch (level)
            {
                case LogLevel.Trace:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Debug:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }

        public void LogTrace(string message, string context = "")
        {
            if (IsEnabled(LogLevel.Trace))
            {
                Log(LogLevel.Trace, message, context, traceColor);
            }
        }

        public void LogDebug(string message, string context = "")
        {
            if (IsEnabled(LogLevel.Debug))
            {
                Log(LogLevel.Debug, message, context, debugColor);
            }
        }

        public void LogWarning(string message, string context = "")
        {
            if (IsEnabled(LogLevel.Warning))
            {
                Log(LogLevel.Warning, message, context, warningColor);
            }
        }

        public void LogError(string message, string context = "")
        {
            if (IsEnabled(LogLevel.Error))
            {
                Log(LogLevel.Error, message, context, errorColor);
            }
        }

        public bool IsEnabled(LogLevel level)
        {
            return (enabledLogLevels & level) != 0;
        }

        private string FormatMessage(LogLevel level, string message, string context, Color color)
        {
            // Example usage of utility methods:
            // Let's say you want different styling for level and context.
            string formattedLevel   = RichTextUtils.FormatText($"[{level}]", color, bold: true);
            string formattedContext = string.IsNullOrEmpty(context) ? "" : RichTextUtils.FormatText(context, color, bold: true);

            // Build the final message
            return $"{formattedLevel} {formattedContext}: {message}";
        }
    }


    /// <summary>
    /// An attribute to mark monobehaviors so they
    /// can be imported into the MB manager editor window
    /// </summary>
    public class ManageableAttribute : Attribute
    {
    }

    /// <summary>
    /// An attribute to mark scriptable objects so they
    /// can be imported into the data manager editor window
    /// </summary>
    public class ManageableDataAttribute : Attribute
    {
    }
}
