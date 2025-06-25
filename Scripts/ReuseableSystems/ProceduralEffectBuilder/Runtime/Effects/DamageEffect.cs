using System.Threading.Tasks;
using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Effects
{
    public class DamageEffect : IEffect, IMagnitudeAware
    {
        private readonly IHealth targetHealth;
        private float damage;

        public DamageEffect(GameObject target, float baseDamage)
        {
            this.targetHealth = target.GetComponent<IHealth>();
            this.damage = baseDamage;
        }

        public void SetMagnitude(float value)
        {
            damage = value;
        }

        public async Task ExecuteAsync()
        {
            targetHealth?.TakeDamage(damage);
            await Task.CompletedTask;
        }

        public IEffect And(IEffect next)
        {
            return new ChainedEffect(this, next);
        }
    }
}
