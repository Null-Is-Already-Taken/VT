using UnityEngine;

namespace VT.Utils
{
    public static class RichTextUtils
    {
        /// <summary>
        /// Returns a string wrapped in optional color, bold, and italic tags for Unity Rich Text.
        /// </summary>
        /// <param name="text">The text to wrap.</param>
        /// <param name="color">Optional color to apply; if omitted, no color tag is added.</param>
        /// <param name="bold">Whether to wrap text in bold tags.</param>
        /// <param name="italic">Whether to wrap text in italic tags.</param>
        /// <returns>A string with Unity Rich Text tags added as specified.</returns>
        public static string FormatText(string text, Color? color = null, bool bold = false, bool italic = false)
        {
            // Start and end tags for optional bold/italic
            string boldStart   = bold   ? "<b>" : "";
            string boldEnd     = bold   ? "</b>" : "";
            string italicStart = italic ? "<i>" : "";
            string italicEnd   = italic ? "</i>" : "";

            // If color is specified, convert it to an HTML string
            string colorStart  = "";
            string colorEnd    = "";
            if (color.HasValue)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(color.Value);
                colorStart = $"<color=#{colorHex}>";
                colorEnd   = "</color>";
            }

            // Compose the final string with any tags
            return $"{boldStart}{italicStart}{colorStart}{text}{colorEnd}{italicEnd}{boldEnd}";
        }
    }
}
