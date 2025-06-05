#if UNITY_EDITOR

using DG.DemiEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;
using VT.IO;
using VT.Utils.Logger;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorWindow : OdinEditorWindow
    {
        private const string CLASS_NAME_MACRO = "#CLASS_NAME#";

        [Title("Script Creator", bold: true)]
        [InfoBox("Paste your C# code below, choose a script name and folder, or load from a text file, then click Save.")]

        [LabelText("Script Name")]
        [Sirenix.OdinInspector.FilePath(AbsolutePath = false, ParentFolder = "Assets")]
        [ValidateInput("@!string.IsNullOrWhiteSpace(scriptName)", "Name cannot be empty")]
        public string scriptName = "NewScript";

        [LabelText("Destination Folder")]
        [FolderPath(AbsolutePath = false, ParentFolder = "Assets")]
        [OnValueChanged(nameof(OnFolderPathChanged))]
        public string folderPath = "Assets/Scripts";

        // —— NEW FILE-LOADER FIELD ——
        [LabelText("Load From .txt File")]
        [Sirenix.OdinInspector.FilePath(Extensions = ".txt", AbsolutePath = false, ParentFolder = "Assets")]
        [OnValueChanged(nameof(OnScriptFilePathChanged))]
        public string scriptFilePath = "VT/Scripts/Tools/ScriptCreator/DefaultScript.txt";

        [LabelText("Script Content")]
        [TextArea(15, 50)]
        public string scriptContent;

        private void OnFolderPathChanged()
        {
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath.TrimStart('/');
            }
        }

        // —— CALLED WHEN YOU PICK OR CHANGE THE .txt PATH ——
        private void OnScriptFilePathChanged()
        {
            var txt = LoadDefaultScript();

            if (txt != null)
            {
                scriptContent = txt;
            }
            else
            {
                InternalLogger.Instance.LogWarning($"[ScriptCreator] File not found at: {scriptFilePath}");
            }
        }

        private string LoadDefaultScript()
        {
            if (string.IsNullOrWhiteSpace(scriptFilePath))
                return null;

            var txt = IOManager.LoadText(scriptFilePath);
            return txt;
        }

        [Button(ButtonSizes.Large)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        public void SaveScript()
        {
            if (string.IsNullOrWhiteSpace(scriptName))
            {
                InternalLogger.Instance.LogError("[ScriptCreator] Script name cannot be empty.");
                return;
            }

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, scriptName + ".cs");

            if (File.Exists(filePath))
            {
                if (!EditorUtility.DisplayDialog(
                    "Overwrite Script?",
                    $"“{scriptName}.cs” already exists. Overwrite?",
                    "Yes", "No"))
                {
                    return;
                }
            }

            if (scriptContent.IsNullOrEmpty())
            {
                var defaultScript = LoadDefaultScript();
                scriptContent = ApplyClassName(defaultScript, scriptName);

                InternalLogger.Instance.LogWarning("<b>Empty script content:</b> Loading default script.");
            }

            if (!scriptContent.IsNullOrEmpty())
            {
                File.WriteAllText(filePath, scriptContent);
                InternalLogger.Instance.LogDebug($"<b>Script saved:</b> {filePath}");
                AssetDatabase.Refresh();
            }
            else
            {
                InternalLogger.Instance.LogError($"<b>Fail to save script...</b>");
            }
        }

        [MenuItem("Tools/Script Creator")]
        private static void OpenWindow()
        {
            var window = GetWindow<ScriptCreatorWindow>();
            Rect windowRect = GUIHelper.GetEditorWindowRect();
            Vector2 vector = new Vector2(450f, Mathf.Min(windowRect.height * 0.7f, 500f));
            Rect position = new Rect(windowRect.center - vector * 0.5f, vector);
            window.position = position;
            window.titleContent = new GUIContent("Script Creator");
        }

        /// <summary>
        /// Replaces every occurrence of the given macro token in the template with the replacement text.
        /// </summary>
        /// <param name="template">The string containing macro tokens (e.g. "#CLASS_NAME#").</param>
        /// <param name="macroToken">The exact token to replace (e.g. "#CLASS_NAME#").</param>
        /// <param name="replacement">What to substitute in place of the macro.</param>
        /// <returns>A new string with all instances of the macro token replaced.</returns>
        public static string ReplaceMacro(string template, string macroToken, string replacement)
        {
            if (string.IsNullOrEmpty(template) || string.IsNullOrEmpty(macroToken))
                return template;
            return template.Replace(macroToken, replacement);
        }

        public static string ApplyClassName(string template, string className)
        {
            return ReplaceMacro(template, CLASS_NAME_MACRO, className);
        }
    }
}

#endif
