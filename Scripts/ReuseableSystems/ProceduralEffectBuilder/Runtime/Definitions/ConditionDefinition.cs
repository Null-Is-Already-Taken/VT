using UnityEngine;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    /// <summary>
    /// Defines a condition that determines whether an effect should be executed.
    /// </summary>
    public abstract class ConditionDefinition : ScriptableObject
    {
        /// <summary>
        /// Evaluates the condition at runtime.
        /// </summary>
        /// <param name="source">The GameObject initiating the effect.</param>
        /// <param name="target">The GameObject that would receive the effect.</param>
        /// <returns>True if the effect should proceed, false if it should be skipped.</returns>
        public abstract bool Evaluate(GameObject source, GameObject target);
    }
}
