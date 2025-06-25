using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Base class for defining how targets are selected.
    /// </summary>
    public abstract class TargetType : ScriptableObject
    {
        /// <summary>
        /// Resolves a list of GameObjects to apply the effect to,
        /// based on the source context (e.g., the caster).
        /// </summary>
        /// <param name="source">The source/caster of the effect.</param>
        /// <returns>A list of resolved targets.</returns>
        public abstract List<GameObject> ResolveTargets(GameObject source);
    }
}
