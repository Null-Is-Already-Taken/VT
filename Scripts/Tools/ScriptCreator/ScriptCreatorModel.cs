#if UNITY_EDITOR && ODIN_INSPECTOR

using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using VT.Extensions;
using VT.IO;
using VT.Logger;

namespace VT.Tools.ScriptCreator
{
    public class ScriptCreatorModel : IEditorModel<ScriptData>
    {
        #region Constants


        private ScriptData data;
        public ScriptData Data => data;

        #endregion

        #region Public Entry Point

        public ScriptData Load()
        {
            throw new NotImplementedException();
        }

        public void SetData(ScriptData data)
        {
            this.data = data;
        }

        public void Save(string path, ScriptData data, bool silentOverwrite = false)
        {
            path = IOManager.NormalizePath(path);

            if (string.IsNullOrWhiteSpace(path))
            {
                InternalLogger.Instance.LogError("[Model] File path cannot be empty.");
                return;
            }

            //var (scriptName, scriptContent) = ProcessScriptContent(data.Content);

            if (IOManager.FileExists(path) && !silentOverwrite)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    title: "Overwrite Script?",
                    message: $"“{data.ClassName}.cs” already exists. Overwrite?",
                    ok: "Yes",
                    cancel: "No"
                );

                if (!overwrite) return;
            }

            try
            {
                IOManager.WriteAllText(path, data.Content);
                InternalLogger.Instance.LogDebug($"[Model] Script {data.ClassName} saved at: {path}");
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                InternalLogger.Instance.LogError($"[Model] Error saving script: {ex.Message}");
            }
        }

        #endregion

        #region Script Processing

        public ScriptData ProcessScriptContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new ScriptData();

            return ScriptDataFactory.FromContent(content);
        }

        #endregion
    }
}

#endif
