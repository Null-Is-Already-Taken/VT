using System;
using UnityEngine;

namespace VT.SkillSystem.Blocks
{
    /// <summary>
    /// Represents the core effect of a skill (damage, healing, buff, etc.)
    /// Can only be followed by Target blocks or Modifier blocks
    /// </summary>
    [Serializable]
    public class EffectBlock : SkillBlock
    {
        [Header("Effect Settings")]
        [SerializeField] private EffectType effectType = EffectType.Damage;
        [SerializeField] private float baseMagnitude = 10f;
        [SerializeField] private bool useContextMagnitude = true;
        
        public enum EffectType
        {
            Damage,
            Healing,
            Buff,
            Debuff,
            Movement,
            Custom
        }
        
        private void OnEnable()
        {
            // Grammar rules: Effect can start a skill, can be followed by Target or Modifiers
            canStartSkill = true;
            validNextBlocks.Add(typeof(TargetBlock));
            validNextBlocks.Add(typeof(MagnitudeModifierBlock));
            validNextBlocks.Add(typeof(RangeModifierBlock));
            validNextBlocks.Add(typeof(DurationModifierBlock));
        }
        
        public override void Execute(SkillContext context)
        {
            float finalMagnitude = useContextMagnitude ? context.Magnitude : baseMagnitude;
            
            switch (effectType)
            {
                case EffectType.Damage:
                    ApplyDamage(context, finalMagnitude);
                    break;
                case EffectType.Healing:
                    ApplyHealing(context, finalMagnitude);
                    break;
                case EffectType.Buff:
                    ApplyBuff(context, finalMagnitude);
                    break;
                case EffectType.Debuff:
                    ApplyDebuff(context, finalMagnitude);
                    break;
                case EffectType.Movement:
                    ApplyMovement(context, finalMagnitude);
                    break;
                case EffectType.Custom:
                    ApplyCustomEffect(context, finalMagnitude);
                    break;
            }
        }
        
        private void ApplyDamage(SkillContext context, float magnitude)
        {
            if (context.Target != null)
            {
                var health = context.Target.GetComponent<IHealth>();
                health?.TakeDamage(magnitude);
            }
        }
        
        private void ApplyHealing(SkillContext context, float magnitude)
        {
            if (context.Target != null)
            {
                var health = context.Target.GetComponent<IHealth>();
                health?.Heal(magnitude);
            }
        }
        
        private void ApplyBuff(SkillContext context, float magnitude)
        {
            // Implementation for buff effects
            Debug.Log($"Applied buff with magnitude {magnitude} to {context.Target?.name}");
        }
        
        private void ApplyDebuff(SkillContext context, float magnitude)
        {
            // Implementation for debuff effects
            Debug.Log($"Applied debuff with magnitude {magnitude} to {context.Target?.name}");
        }
        
        private void ApplyMovement(SkillContext context, float magnitude)
        {
            // Implementation for movement effects
            Debug.Log($"Applied movement effect with magnitude {magnitude}");
        }
        
        private void ApplyCustomEffect(SkillContext context, float magnitude)
        {
            // Custom effect implementation
            Debug.Log($"Applied custom effect with magnitude {magnitude}");
        }
    }
    
    // Simple health interface for demonstration
    public interface IHealth
    {
        void TakeDamage(float damage);
        void Heal(float amount);
    }
} 