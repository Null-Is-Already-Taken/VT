#if UNITY_EDITOR && ODIN_INSPECTOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VT.IO;
using VT.Logger;

namespace VT.Tools.UITKConstantGenerator
{
    public class UITKConstantGeneratorWindow : OdinEditorWindow
    {
        [Title("UI Constant Generator", bold: true)]
        [InfoBox("Choose a folder containing .uxml files. Preview the constants before saving.")]

        [FolderPath(AbsolutePath = false)]
        [ValidateInput("@System.IO.Directory.Exists(targetFolderPath)", "Folder must exist")]
        [LabelText("Target Folder")]
        public string targetFolderPath = "Assets/UI";

        // For toggling the use of UITKConstant wrapper
        private bool useWrapperConstant = false;

        [Button("Generate Preview", ButtonSizes.Large)]
        [GUIColor(0.2f, 0.6f, 1f)]
        public void GeneratePreview()
        {
            previewItems.Clear();

            string[] uxmlFiles = Directory.GetFiles(targetFolderPath, "*.uxml", SearchOption.AllDirectories);
            if (uxmlFiles.Length == 0)
            {
                InternalLogger.Instance.LogWarning($"[UIConstantGen] No UXML files found in: {targetFolderPath}");
                return;
            }

            foreach (string uxmlPathRaw in uxmlFiles)
            {
                string uxmlPath = IOManager.NormalizePath(uxmlPathRaw);
                string fileName = Path.GetFileNameWithoutExtension(uxmlPath);
                string className = UITKConstantGeneratorAPI.SanitizeClassName(fileName);
                string uxmlFolder = Path.GetDirectoryName(uxmlPath);
                string outputPath = Path.Combine(uxmlFolder, className + ".cs");
                outputPath = IOManager.NormalizePath(outputPath);

                string generatedContent = UITKConstantGeneratorAPI.GenerateClassContent(uxmlPath, className, useWrapperConstant: useWrapperConstant);
                previewItems.Add(new UITKCodePreview(uxmlPath, outputPath, generatedContent));
            }

            InternalLogger.Instance.LogDebug($"[UIConstantGen] Preview generated for {previewItems.Count} files.");
        }

        [ShowInInspector, LabelText("Preview")]
        [ListDrawerSettings(DraggableItems = false, ShowIndexLabels = false)]
        private List<UITKCodePreview> previewItems = new();

        [Button("Clear All", ButtonSizes.Large)]
        [GUIColor(1.0f, 0.9f, 0.5f)]
        [EnableIf("@previewItems.Count > 0")]
        [PropertyOrder(11)]
        public void ClearAll()
        {
            previewItems.Clear();
            InternalLogger.Instance.LogDebug("[UIConstantGen] Preview cleared.");
        }

        [Button("Apply All", ButtonSizes.Large)]
        [GUIColor(0.3f, 0.9f, 0.3f)]
        [EnableIf("@previewItems.Count > 0")]
        [PropertyOrder(11)]
        public void ApplyAll()
        {
            var trackingData = UITKConstantTrackerHelper.LoadTrackingData();

            foreach (var item in previewItems)
            {
                File.WriteAllText(item.OutputPath, item.GeneratedCode);
                InternalLogger.Instance.LogDebug($"[UIConstantGen] Saved: {item.OutputPath}");

                trackingData[item.UxmlPath] = item.OutputPath;
            }

            UITKConstantTrackerHelper.SaveTrackingData(trackingData);
            AssetDatabase.Refresh();
            InternalLogger.Instance.LogDebug("[UIConstantGen] All constants saved.");
        }

        [MenuItem("Tools/UI Constant Generator")]
        private static void OpenWindow()
        {
            var window = GetWindow<UITKConstantGeneratorWindow>();
            window.titleContent = new GUIContent("UI Constant Generator");
        }
    }
}

#endif
