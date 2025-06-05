using UnityEngine;

namespace VT.Generic
{
    public static class Boundary3DUtils
    {
        public static Vector3 GetCenter(Boundary3D boundary) => (boundary.Min + boundary.Max) * 0.5f;
        public static Vector3 GetSize(Boundary3D boundary) => boundary.Max - boundary.Min;

        public static Vector3 GetFaceCenter(Boundary3D boundary, Axis axis, bool positive)
        {
            Vector3 center = GetCenter(boundary);
            Vector3 min = boundary.Min;
            Vector3 max = boundary.Max;

            return axis switch
            {
                Axis.X => new Vector3(positive ? max.x : min.x, center.y, center.z),
                Axis.Y => new Vector3(center.x, positive ? max.y : min.y, center.z),
                Axis.Z => new Vector3(center.x, center.y, positive ? max.z : min.z),
                _ => center,
            };
        }

        public static Vector3[] GetBoxFace(Boundary3D boundary)
        {
            Vector3 center = GetCenter(boundary);
            Vector3 size = GetSize(boundary);
            Vector3 half = size * 0.5f;
            return new Vector3[]
            {
                center + new Vector3(-half.x, -half.y, -half.z),
                center + new Vector3(-half.x,  half.y, -half.z),
                center + new Vector3( half.x,  half.y, -half.z),
                center + new Vector3( half.x, -half.y, -half.z)
            };
        }

        public enum Axis { X, Y, Z }
    }
}
