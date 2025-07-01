using System;
using UnityEngine;

namespace VT.SkillSystem.Blocks
{
    /// <summary>
    /// Modifies the range of the skill
    /// Can follow Effect, Target, or other Modifier blocks
    /// </summary>
    [Serializable]
    public class RangeModifierBlock : SkillBlock
    {
        [Header("Range Settings")]
        [SerializeField] private ModifierType modifierType = ModifierType.Multiply;
        [SerializeField] private float modifierValue = 1.5f;
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
            validNextBlocks.Add(typeof(DurationModifierBlock));
        }
        
        public override void Execute(SkillContext context)
        {
            float currentRange = context.Range;
            float finalValue = modifierValue;
            
            if (isPercentage)
            {
                finalValue = modifierValue / 100f;
            }
            
            switch (modifierType)
            {
                case ModifierType.Add:
                    context.Range = currentRange + finalValue;
                    break;
                case ModifierType.Multiply:
                    context.Range = currentRange * finalValue;
                    break;
                case ModifierType.Set:
                    context.Range = finalValue;
                    break;
            }
            
            Debug.Log($"Range modified: {currentRange} -> {context.Range}");
        }
    }
} 