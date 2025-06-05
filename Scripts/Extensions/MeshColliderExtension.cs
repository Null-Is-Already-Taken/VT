using UnityEngine;

namespace VT.Extensions
{
    public static class MeshColliderExtentions
    {
        public static Vector3 GetRandomPointInMesh(this MeshCollider meshCollider)
        {
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return meshCollider != null ? meshCollider.bounds.center : Vector3.zero;

            Mesh mesh = meshCollider.sharedMesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            // Pick a random triangle
            int triIndex = Random.Range(0, triangles.Length / 3) * 3;
            Vector3 v0 = meshCollider.transform.TransformPoint(vertices[triangles[triIndex]]);
            Vector3 v1 = meshCollider.transform.TransformPoint(vertices[triangles[triIndex + 1]]);
            Vector3 v2 = meshCollider.transform.TransformPoint(vertices[triangles[triIndex + 2]]);

            // Generate a random point in the triangle using barycentric coordinates
            float a = Random.value;
            float b = Random.value;
            if (a + b > 1f)
            {
                a = 1f - a;
                b = 1f - b;
            }
            float c = 1f - a - b;
            Vector3 randomPoint = a * v0 + b * v1 + c * v2;
            return randomPoint;
        }
    }
}
