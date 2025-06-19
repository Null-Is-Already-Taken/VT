namespace VT.Extensions
{
    public static class StringExtensions
    {
        public static string Indents(this string text, int level = 1)
        {
            return new string('\t', level) + text;
        }
    }
}
