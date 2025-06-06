// Preview data class
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace VT.Tools.UITKConstantGenerator
{
    public class UITKCodePreview
    {
        [ReadOnly] public string UxmlPath;
        [ReadOnly] public string OutputPath;
        [HideInInspector] public string GeneratedCode;

        public string File => Path.GetFileName(UxmlPath);
        [TextArea(15, 30)] public string Code;

        public UITKCodePreview(string uxmlPath, string outputPath, string code)
        {
            UxmlPath = uxmlPath;
            OutputPath = outputPath;
            GeneratedCode = code;
            Code = code;
        }
    }
}