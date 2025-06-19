using System;

namespace VT.Tools.ScriptCreator
{
    [Serializable]
    public struct ScriptData
    {
        public ScriptData(string className, string content)
        {
            this.className = className;
            this.content = content;
        }

        private string className;
        /// <summary>The desired class name to insert into the script.</summary>
        public string ClassName => className;


        private string content;
        /// <summary>The raw script content, may include a macro token.</summary>
        public string Content => content;
    }
}
