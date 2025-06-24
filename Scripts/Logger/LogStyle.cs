using System;

namespace VT.Logger
{
    /// <summary>
    /// Represents a style for formatting log messages.
    /// </summary>
    /// <remarks>
    /// This class allows you to define how log messages should be styled, including prefixes, suffixes,
    /// uppercase transformation, and custom formatting functions.
    /// </remarks>
    public class LogStyle
    {
        public string PrefixText { get; private set; } = "";
        public string SuffixText { get; private set; } = "";
        public bool Uppercase { get; private set; } = false;
        public Func<string, string> CustomFormatter { get; private set; }

        public static LogStyle Prefix(string prefix) => new() { PrefixText = prefix };
        public static LogStyle Suffix(string suffix) => new() { SuffixText = suffix };
        public static LogStyle Upper() => new() { Uppercase = true };
        public static LogStyle Format(Func<string, string> formatter) => new() { CustomFormatter = formatter };

        public LogStyle Merge(LogStyle other)
        {
            return new LogStyle
            {
                PrefixText = this.PrefixText + other.PrefixText,
                SuffixText = this.SuffixText + other.SuffixText,
                Uppercase = this.Uppercase || other.Uppercase,
                CustomFormatter = this.CustomFormatter != null && other.CustomFormatter != null
                    ? (s => other.CustomFormatter(this.CustomFormatter(s)))
                    : this.CustomFormatter ?? other.CustomFormatter
            };
        }

        public static LogStyle Compose(params LogStyle[] styles)
        {
            var composed = new LogStyle();
            foreach (var style in styles)
            {
                if (style != null)
                    composed = composed.Merge(style);
            }
            return composed;
        }

        public string Apply(string input)
        {
            string result = $"{PrefixText}{input}{SuffixText}";
            if (Uppercase) result = result.ToUpperInvariant();
            if (CustomFormatter != null) result = CustomFormatter(result);
            return result;
        }
    }
}
