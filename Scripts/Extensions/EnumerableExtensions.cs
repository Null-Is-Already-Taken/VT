using System.Collections.Generic;
using UnityEngine;

namespace VT.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Prints each item on its own line using the internal Extensions logger.
        /// Optional label becomes the log 'context' (e.g., collection name).
        /// </summary>
        public static void Print<T>(
            this IEnumerable<T> collection,
            string label = null,
            LogLevel level = LogLevel.Debug)
        {
            if (collection == null) return;

            var logger = ExtensionsLogger.Logger;
            int index = 0;

            foreach (var item in collection)
            {
                string text = item?.ToString() ?? "null";
                // Example format: add index prefix for readability
                string line = $"[{index++}] {text}";

                switch (level)
                {
                    case LogLevel.Trace: logger.LogTrace(line, label); break;
                    case LogLevel.Warning: logger.LogWarning(line, label); break;
                    case LogLevel.Error: logger.LogError(line, label); break;
                    default: logger.LogDebug(line, label); break;
                }
            }
        }
    }
}
