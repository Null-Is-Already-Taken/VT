using System;
using UnityEngine;

namespace VT.Generic
{
    [Serializable]
    public struct Boundary2D
    {
        [SerializeField] private Vector2 min;
        public readonly Vector2 Min => min;

        [SerializeField] private Vector2 max;
        public readonly Vector2 Max => max;

        public Boundary2D(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public readonly Vector2 Center => (Min + Max) / 2f;
        public readonly Vector2 Size => Max - Min;

        public readonly bool Contains(Vector2 point)
        {
            return point.x >= Min.x && point.x <= Max.x &&
                point.y >= Min.y && point.y <= Max.y;
        }

        public readonly bool Intersects(Boundary2D other)
        {
            return
            !(
                other.Min.x > Max.x || other.Max.x < Min.x ||
                other.Min.y > Max.y || other.Max.y < Min.y
            );
        }

        public override readonly string ToString()
        {
            return $"Boundary2D(Min: {Min}, Max: {Max})";
        }
    }
}
