using System;
using System.Collections.Generic;

namespace VT.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly Random GlobalRandom = new(); // For generating random seeds

        // Generic shuffle for IList<T>, returns the seed used
        public static int Shuffle<T>(this IList<T> list, int? seed = null)
        {
            seed ??= GlobalRandom.Next(); // Generate a seed if not provided
            Random random = new(seed.Value);
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return seed.Value;
        }

        // Overload for arrays specifically, returns the seed used
        public static int Shuffle<T>(this T[] array, int? seed = null)
        {
            seed ??= GlobalRandom.Next(); // Generate a seed if not provided
            Random random = new(seed.Value);
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
            return seed.Value;
        }
    }
}
