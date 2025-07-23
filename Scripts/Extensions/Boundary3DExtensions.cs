using UnityEngine;
using VT.Generic;

namespace VT.Extensions
{
    public static class Boundary3DExtensions
    {
        /// <summary>
        /// Returns a random point within the Boundary3D.
        /// </summary>
        public static Vector3 GetRandomPoint(this Boundary3D boundary)
        {
            float x = Random.Range(boundary.Min.x, boundary.Max.x);
            float y = Random.Range(boundary.Min.y, boundary.Max.y);
            float z = Random.Range(boundary.Min.z, boundary.Max.z);
            return new Vector3(x, y, z);
        }
    }
} 