using System;
using UnityEngine;

namespace VT.Generic
{
    [Serializable]
    public struct Boundary3D
    {
        [SerializeField] private Vector3 min;
        public readonly Vector3 Min => min;

        [SerializeField] private Vector3 max;
        public readonly Vector3 Max => max;

        public Boundary3D(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public readonly Vector3 Center => (min + max) / 2f;
        public readonly Vector3 Size => max - min;

        public readonly bool Contains(Vector3 point)
        {
            return point.x >= min.x && point.x <= max.x &&
                point.y >= min.y && point.y <= max.y &&
                point.z >= min.z && point.z <= max.z;
        }


        public readonly bool Intersects(Boundary3D other)
        {
            return
            !(
                other.Min.x > max.x || other.Max.x < min.x ||
                other.Min.y > max.y || other.Max.y < min.y ||
                other.Min.z > max.z || other.Max.z < min.z
            );
        }

        public override readonly string ToString()
        {
            return $"Boundary3D(Min: {min}, Max: {max})";
        }
    }
}
