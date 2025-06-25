using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Effects
{
    /// <summary>
    /// A simple effect definition that causes flat or scalable damage.
    /// </summary>
    [CreateAssetMenu(menuName = "Procedural Effect Builder/Effects/Damage")]
    public class DamageEffectDefinition : EffectDefinition
    {
        [Tooltip("Default damage value if magnitude is not injected.")]
        public float baseDamage = 10f;

        public override IEffect Compile(GameObject source, GameObject target)
        {
            return new DamageEffect(target, baseDamage);
        }
    }
}
