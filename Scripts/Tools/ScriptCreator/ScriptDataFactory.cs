#if UNITY_EDITOR

using System.Text;
using System.Text.RegularExpressions;
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
            return new ScriptData
            (
                className: ExtractClassName(content),
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

        public static string ExtractClassName(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var match = Regex.Match(content, @"\b(?:class|interface|struct|enum)\s+([A-Za-z_][A-Za-z0-9_]*)");
            return match.Success ? match.Groups[1].Value : DEFAULT_NAME;
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
