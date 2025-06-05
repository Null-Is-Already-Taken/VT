#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VT.Generic;

namespace VT.Visualization
{
    [InitializeOnLoad]
    public static class Boundary3DSceneDrawer
    {
        // Colors to cycle through
        private static readonly Color[] BoundaryColors = {
            Color.red, Color.green, Color.blue, Color.yellow,
            Color.magenta, Color.cyan, new Color(1f, 0.5f, 0f), new Color(0.5f, 0f, 1f)
        };

        // Toggle for face handles
        private static bool showHandles = true;

        static Boundary3DSceneDrawer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Application.isPlaying) return;

            // Gather all selected MonoBehaviours that have a serialized Boundary3D field
            var entries = GatherSelectedBoundaries(out bool anyBoundary);
            if (anyBoundary) DrawToolbar();

            int colorIndex = 0;
            foreach (var entry in entries)
            {
                var color = BoundaryColors[colorIndex++ % BoundaryColors.Length];
                DrawBoundary(entry.mono, entry.field, entry.boundary, color);
            }
        }

        private struct BoundaryEntry
        {
            public MonoBehaviour mono;
            public FieldInfo field;
            public Boundary3D boundary;
        }

        private static List<BoundaryEntry> GatherSelectedBoundaries(out bool anyBoundary)
        {
            var list = new List<BoundaryEntry>();
            foreach (var go in Selection.gameObjects)
            {
                foreach (var mono in go.GetComponents<MonoBehaviour>())
                {
                    foreach (var field in mono.GetType()
                                            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (!IsSerialized(field) || field.FieldType != typeof(Boundary3D))
                            continue;

                        var boundary = (Boundary3D)field.GetValue(mono);
                        list.Add(new BoundaryEntry { mono = mono, field = field, boundary = boundary });
                    }
                }
            }
            anyBoundary = list.Count > 0;
            return list;
        }

        private static bool IsSerialized(FieldInfo field)
            => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;

        private static void DrawToolbar()
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, 10, 200, 30), EditorStyles.helpBox);
            string label = showHandles ? "Hide" : "Show";
            showHandles = GUILayout.Toggle(showHandles, $"{label} Handle Caps", EditorStyles.toolbarButton);
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private static void DrawBoundary(MonoBehaviour mono, FieldInfo field, Boundary3D boundary, Color color)
        {
            var t = mono.transform;
            var center = Boundary3DUtils.GetCenter(boundary);
            var size   = Boundary3DUtils.GetSize(boundary);

            var matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            using (new Handles.DrawingScope(matrix))
            {
                // Draw the box
                Handles.color = color;
                Handles.DrawWireCube(center, size);
                Handles.color = new Color(color.r, color.g, color.b, 0.05f);
                Handles.DrawSolidRectangleWithOutline(Boundary3DUtils.GetBoxFace(boundary),
                                                    new Color(color.r, color.g, color.b, 0.05f),
                                                    color);

                if (showHandles)
                    DrawFaceHandles(mono, field, boundary, color, center);
            }
        }

        private static void DrawFaceHandles(MonoBehaviour mono,
                                            FieldInfo field,
                                            Boundary3D oldBoundary,
                                            Color color,
                                            Vector3 center)
        {
            float hSize = HandleUtility.GetHandleSize(center) * 0.05f;
            Handles.color = color;

            EditorGUI.BeginChangeCheck();
            Vector3 newXMin = Handles.Slider(
                Boundary3DUtils.GetFaceCenter(oldBoundary, Boundary3DUtils.Axis.X, false),
                Vector3.right, hSize, Handles.DotHandleCap, 0.1f);
            Vector3 newXMax = Handles.Slider(
                Boundary3DUtils.GetFaceCenter(oldBoundary, Boundary3DUtils.Axis.X, true),
                Vector3.right, hSize, Handles.DotHandleCap, 0.1f);
            Vector3 newYMin = Handles.Slider(
                Boundary3DUtils.GetFaceCenter(oldBoundary, Boundary3DUtils.Axis.Y, false),
                Vector3.up, hSize, Handles.DotHandleCap, 0.1f);
            Vector3 newYMax = Handles.Slider(
                Boundary3DUtils.GetFaceCenter(oldBoundary, Boundary3DUtils.Axis.Y, true),
                Vector3.up, hSize, Handles.DotHandleCap, 0.1f);
            Vector3 newZMin = Handles.Slider(
                Boundary3DUtils.GetFaceCenter(oldBoundary, Boundary3DUtils.Axis.Z, false),
                Vector3.forward, hSize, Handles.DotHandleCap, 0.1f);
            Vector3 newZMax = Handles.Slider(
                Boundary3DUtils.GetFaceCenter(oldBoundary, Boundary3DUtils.Axis.Z, true),
                Vector3.forward, hSize, Handles.DotHandleCap, 0.1f);

            if (!EditorGUI.EndChangeCheck())
                return;

            Undo.RecordObject(mono, "Edit Boundary3D Face");

            var newBoundary = new Boundary3D(
                new Vector3(newXMin.x, newYMin.y, newZMin.z),
                new Vector3(newXMax.x, newYMax.y, newZMax.z)
            );

            field.SetValue(mono, newBoundary);
            EditorUtility.SetDirty(mono);
        }
    }
}
#endif
