using System;
using System.Collections.Generic;
using UnityEngine;
using VT.Logger;

namespace VT.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Print<T>(
            this IEnumerable<T> collection,
            params LogStyle[] styles)
        {
            if (collection == null)
            {
                Debug.Log("Collection is null");
                return;
            }

            var style = LogStyle.Compose(styles);

            foreach (var item in collection)
            {
                string output = item?.ToString();
                InternalLogger.Instance.LogDebug(style?.Apply(output) ?? output);
            }
        }
    }
}
