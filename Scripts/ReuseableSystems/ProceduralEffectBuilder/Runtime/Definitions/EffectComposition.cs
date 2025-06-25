using System.Collections.Generic;
using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Defines a higher-level structure for composing multiple effects together.
    /// Used to override the default single effect logic in SkillTemplate.
    /// </summary>
    public abstract class EffectComposition : ScriptableObject
    {
        [Tooltip("Sub-effects to compile and combine.")]
        public List<EffectDefinition> subEffects;

        /// <summary>
        /// Compiles the composition into a runtime IEffect.
        /// </summary>
        /// <param name="source">The source/caster of the skill.</param>
        /// <param name="target">The single or default target (optional).</param>
        /// <returns>A composed IEffect (e.g. sequence, parallel, branch).</returns>
        public abstract IEffect Compile(GameObject source, GameObject target);
    }
}
