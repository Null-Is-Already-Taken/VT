using System;
using System.Collections.Generic;

namespace VT.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>(this T e) where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
