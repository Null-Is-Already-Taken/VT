// --- SkillLogger.cs ---
using UnityEngine;
using System.Text;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class SkillLogger : ISkillLogger
    {
        private readonly StringBuilder buffer = new();
        private readonly string skillName;

        public SkillLogger(string skillName)
        {
            this.skillName = skillName;
            Log($"<b>--- Executing Skill: {skillName} ---</b>");
        }

        public void Log(string message)
        {
            buffer.AppendLine(message);
            Debug.Log(message);
        }

        public void LogTag(string tag, string message)
        {
            Log($"[{tag}] {message}");
        }

        public override string ToString() => buffer.ToString();
    }
}