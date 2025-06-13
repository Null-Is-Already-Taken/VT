#if UNITY_EDITOR
using UnityEngine;

namespace VT.Editor.Utils
{
    /// <summary>
    /// Generic string measurement and truncation helpers.
    /// </summary>
    public static class TextUtils
    {
        public static string TruncateWithEllipsis(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length <= maxLength) return text;
            int len = System.Math.Max(maxLength - 3, 0);
            return text.Substring(0, len) + "...";
        }

        public static int EstimateMaxChars(float width, float averageCharWidth) => Mathf.FloorToInt(width / averageCharWidth);
    }
}
#endif
