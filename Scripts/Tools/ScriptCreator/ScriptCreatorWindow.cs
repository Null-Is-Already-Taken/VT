#if UNITY_EDITOR

using DG.DemiEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        [ReadOnly]
        //[ValidateInput("@!string.IsNullOrWhiteSpace(scriptName)", "Name cannot be empty")]
        public string scriptName = "NewScript";

        [LabelText("Destination Folder")]
        [FolderPath(AbsolutePath = false)]
        [ValidateInput("@!string.IsNullOrWhiteSpace(folderPath)", "Path cannot be empty")]
        public string folderPath = "Assets/Scripts";

        [LabelText("Script Content")]
        [TextArea(15, 50)]
        [OnValueChanged(nameof(OnScriptContentChanged))]
        public string scriptContent;

        private void LoadDefaultScript()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine($"public class {CLASS_NAME_MACRO} : MonoBehaviour");
            sb.AppendLine("{");
            sb.AppendLine($"{Tab()}private void Start()");
            sb.AppendLine($"{Tab()}{{");
            sb.AppendLine();
            sb.AppendLine($"{Tab()}}}");
            sb.AppendLine();
            sb.AppendLine($"{Tab()}private void Update()");
            sb.AppendLine($"{Tab()}{{");
            sb.AppendLine();
            sb.AppendLine($"{Tab()}}}");
            sb.AppendLine("}");

            scriptContent = sb.ToString();
        }

        private void OnScriptContentChanged()
        {
            // Automatically extract class name from script content
            if (!string.IsNullOrWhiteSpace(scriptContent))
            {
                scriptName = ExtractClassName(scriptContent);
            }

            if (scriptName == CLASS_NAME_MACRO)
            {
                scriptName = "NewScript"; // Default name if macro is used
            }

            scriptContent = ApplyClassName(scriptContent, scriptName);
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

            if (!IOManager.DirectoryExists(folderPath))
                IOManager.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, scriptName + ".cs");

            if (IOManager.FileExists(filePath))
            {
                if (!EditorUtility.DisplayDialog(
                    "Overwrite Script?",
                    $"“{scriptName}.cs” already exists. Overwrite?",
                    "Yes", "No"))
                {
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(scriptContent))
            {
                LoadDefaultScript();
                scriptContent = ApplyClassName(scriptContent, scriptName);

                InternalLogger.Instance.LogWarning("<b>Empty script content:</b> Loading default script.");
            }

            if (!scriptContent.IsNullOrEmpty())
            {
                IOManager.SaveText(filePath, scriptContent);
                InternalLogger.Instance.LogDebug($"<b>Script saved:</b> {filePath}");
                AssetDatabase.Refresh();
            }
            else
            {
                InternalLogger.Instance.LogError($"<b>Fail to save script...</b>");
            }
        }

        [Button(ButtonSizes.Large)]
        private void TestIO()
        {
            //InternalLogger.Instance.LogDebug(this.folderPath);
            //InternalLogger.Instance.LogDebug(IOManager.ResolvePath(this.folderPath));
            //InternalLogger.Instance.LogDebug(IOManager.ResolvePath("#persistent/" + this.folderPath));
            //InternalLogger.Instance.LogDebug(IOManager.ResolvePath("#data/" + this.folderPath));
            //InternalLogger.Instance.LogDebug(IOManager.ResolvePath("#streaming/" + this.folderPath));
            //InternalLogger.Instance.LogDebug(IOManager.ResolvePath("#temp/" + this.folderPath));
            //InternalLogger.Instance.LogDebug(IOManager.ResolvePath("#project/" + this.folderPath));

            //var folderPath = "#persistent/" + this.folderPath;
            //InternalLogger.Instance.LogDebug($"Directory {folderPath} exists:" + IOManager.DirectoryExists(folderPath));
        }

        [MenuItem("Tools/Script Creator")]
        private static void OpenWindow()
        {
            var window = GetWindow<ScriptCreatorWindow>();
            Rect windowRect = GUIHelper.GetEditorWindowRect();
            Vector2 vector = new(450f, Mathf.Min(windowRect.height * 0.7f, 500f));
            Rect position = new(windowRect.center - vector * 0.5f, vector);
            window.position = position;
            window.titleContent = new GUIContent("Script Creator");
            window.LoadDefaultScript(); // Load default script on open
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

        public static string ExtractClassName(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                return "NewScript";

            // Match "class ClassName" optionally preceded by modifiers like public, private, etc.
            var match = Regex.Match(script, @"\bclass\s+([A-Za-z_][A-Za-z0-9_]*)");

            if (match.Success && match.Groups.Count > 1)
                return match.Groups[1].Value;

            return "NewScript";
        }

        public static string ApplyClassName(string template, string className)
        {
            return ReplaceMacro(template, CLASS_NAME_MACRO, className);
        }

        private static string Tab(int tabCount = 1)
        {
            return new string('\t', tabCount);
        }
    }
}

#endif
