using UnityEngine;
using VT.Generic;

namespace VT.Extensions
{
    public static class Boundary2DExtensions
    {
        /// <summary>
        /// Returns a random point within the Boundary2D.
        /// </summary>
        public static Vector2 GetRandomPoint(this Boundary2D boundary)
        {
            float x = Random.Range(boundary.Min.x, boundary.Max.x);
            float y = Random.Range(boundary.Min.y, boundary.Max.y);
            return new Vector2(x, y);
        }
    }
} 