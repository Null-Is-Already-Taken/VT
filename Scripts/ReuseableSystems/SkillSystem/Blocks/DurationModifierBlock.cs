using System;
using UnityEngine;

namespace VT.SkillSystem.Blocks
{
    /// <summary>
    /// Modifies the duration of the skill effect
    /// Can follow Effect, Target, or other Modifier blocks
    /// </summary>
    [Serializable]
    public class DurationModifierBlock : SkillBlock
    {
        [Header("Duration Settings")]
        [SerializeField] private ModifierType modifierType = ModifierType.Multiply;
        [SerializeField] private float modifierValue = 2f;
        [SerializeField] private bool isPercentage = false;
        
        public enum ModifierType
        {
            Add,
            Multiply,
            Set
        }
        
        private void OnEnable()
        {
            // Grammar rules: Modifier can follow Effect, Target, or other Modifier blocks
            isModifier = true;
            validPreviousBlocks.Add(typeof(EffectBlock));
            validPreviousBlocks.Add(typeof(TargetBlock));
            validPreviousBlocks.Add(typeof(MagnitudeModifierBlock));
            validPreviousBlocks.Add(typeof(RangeModifierBlock));
        }
        
        public override void Execute(SkillContext context)
        {
            float currentDuration = context.Duration;
            float finalValue = modifierValue;
            
            if (isPercentage)
            {
                finalValue = modifierValue / 100f;
            }
            
            switch (modifierType)
            {
                case ModifierType.Add:
                    context.Duration = currentDuration + finalValue;
                    break;
                case ModifierType.Multiply:
                    context.Duration = currentDuration * finalValue;
                    break;
                case ModifierType.Set:
                    context.Duration = finalValue;
                    break;
            }
            
            Debug.Log($"Duration modified: {currentDuration} -> {context.Duration}");
        }
    }
} 