#if UNITY_EDITOR

using System.Text;
using VT.Extensions;

namespace VT.Tools.ScriptCreator
{
    public static class ScriptDataFactory
    {
        public const string DEFAULT_NAME = "NewScript";

        public static ScriptData CreateDefault()
        {
            var content = LoadDefaultScript();
            return new ScriptData
            (
                className: DEFAULT_NAME,
                content: content
            );
        }

        public static ScriptData FromContent(string content)
        {
            string className = TypeNameExtractor.ExtractClassName(content);
            if (string.IsNullOrWhiteSpace(className))
            {
                className = DEFAULT_NAME;
            }

            return new ScriptData
            (
                className: className,
                content: content
            );
        }

        public static string LoadDefaultScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine($"public class {DEFAULT_NAME} : MonoBehaviour");
            sb.AppendLine("{");
            sb.AppendLine("private void Start()".Indents());
            sb.AppendLine("{".Indents());
            sb.AppendLine("}".Indents());
            sb.AppendLine();
            sb.AppendLine("private void Update()".Indents());
            sb.AppendLine("{".Indents());
            sb.AppendLine("}".Indents());
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static ScriptData ProcessScriptContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new ScriptData();

            return FromContent(content);
        }
    }
}

#endif
