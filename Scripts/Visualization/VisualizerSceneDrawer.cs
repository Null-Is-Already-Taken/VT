using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace VT.Visualization
{
    [InitializeOnLoad]
    public static class VisualizerSceneDrawer
    {
        static VisualizerSceneDrawer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Application.isPlaying) return;

            foreach (var mono in Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.None))
            {
                if (mono is IVisualizer visualizer)
                    visualizer.DrawGizmos();

                if (mono is IDraggable draggable)
                {
                    Handles.color = Color.magenta;
                    draggable.DrawHandles();
                }
            }
        }
    }
}
#endif
