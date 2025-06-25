using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Base class for all effect definitions.
    /// Designers create custom effects by inheriting from this.
    /// At runtime, this gets compiled into a concrete IEffect.
    /// </summary>
    public abstract class EffectDefinition : ScriptableObject
    {
        /// <summary>
        /// Compiles the design-time definition into a runtime IEffect.
        /// </summary>
        /// <param name="source">The object applying the effect (caster).</param>
        /// <param name="target">The object receiving the effect.</param>
        /// <returns>Compiled IEffect instance.</returns>
        public abstract IEffect Compile(GameObject source, GameObject target);
    }
}
