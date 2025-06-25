using UnityEngine;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Base class for resolving the source (caster) of a skill effect.
    /// </summary>
    public abstract class SourceType : ScriptableObject
    {
        /// <summary>
        /// Resolves the origin or caster of the skill.
        /// </summary>
        /// <param name="context">The context object passed from the runtime (e.g., event instigator).</param>
        /// <returns>The resolved source GameObject.</returns>
        public abstract GameObject ResolveSource(GameObject context);
    }
}
