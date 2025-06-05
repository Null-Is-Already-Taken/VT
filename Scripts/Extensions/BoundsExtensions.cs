using System.Runtime.CompilerServices;
using UnityEngine;

namespace VT.Extensions
{
    public static class BoundsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectsXY(this Bounds bounds, Bounds other)
        {
            return bounds.min.x <= other.max.x && bounds.max.x >= other.min.x && bounds.min.y <= other.max.y && bounds.max.y >= other.min.y;
        }
    }
}