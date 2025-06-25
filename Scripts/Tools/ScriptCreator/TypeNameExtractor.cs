//using System.Linq;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//public static class TypeNameExtractor
//{
//    private const string FallbackName = "GeneratedType";   // your DEFAULT_NAME

//    public static string ExtractFirstDeclaredType(string content)
//    {
//        if (string.IsNullOrWhiteSpace(content))
//            return string.Empty;

//        // Parse exactly like the compiler
//        var tree  = CSharpSyntaxTree.ParseText(content);
//        var root  = tree.GetRoot();

//        // DescendantNodes() returns nodes in source-order, so the first match is
//        // the one that actually appears first in the file.
//        var first = root.DescendantNodes()
//                        .FirstOrDefault(n =>
//                             n is BaseTypeDeclarationSyntax        // class / struct / interface / enum
//                          || n is RecordDeclarationSyntax);        // record / record struct

//        return first switch
//        {
//            ClassDeclarationSyntax     c => c.Identifier.Text,
//            StructDeclarationSyntax    s => s.Identifier.Text,
//            InterfaceDeclarationSyntax i => i.Identifier.Text,
//            EnumDeclarationSyntax      e => e.Identifier.Text,
//            RecordDeclarationSyntax    r => r.Identifier.Text,
//            _                            => FallbackName
//        };
//    }
//}

using System.Text.RegularExpressions;

namespace VT.Tools.ScriptCreator
{
    public static class TypeNameExtractor
    {
        private static readonly Regex _commentStripper = new(
            @"//.*?$            # single-line comments
            | /\*.*?\*/         # block comments
            | ^\s*///.*?$       # XML docs
        ",
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _typeDecl = new(
            @"\b(class|interface|struct|enum)\s+([A-Za-z_][A-Za-z0-9_]*)",
            RegexOptions.Compiled);

        public static string ExtractClassName(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // 1. Strip comments & docs
            string code = _commentStripper.Replace(content, "");

            // 2. Find the first type declaration
            var m = _typeDecl.Match(code);
            return m.Success ? m.Groups[2].Value : null;
        }
    }
}