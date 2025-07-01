using System;
using UnityEngine;

namespace VT.SkillSystem.Blocks
{
    /// <summary>
    /// Modifies the magnitude of the skill effect
    /// Can follow Effect or Target blocks
    /// </summary>
    [Serializable]
    public class MagnitudeModifierBlock : SkillBlock
    {
        [Header("Magnitude Settings")]
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
            // Grammar rules: Modifier can follow Effect or Target blocks
            isModifier = true;
            validPreviousBlocks.Add(typeof(EffectBlock));
            validPreviousBlocks.Add(typeof(TargetBlock));
            validNextBlocks.Add(typeof(RangeModifierBlock));
            validNextBlocks.Add(typeof(DurationModifierBlock));
        }
        
        public override void Execute(SkillContext context)
        {
            float currentMagnitude = context.Magnitude;
            float finalValue = modifierValue;
            
            if (isPercentage)
            {
                finalValue = modifierValue / 100f;
            }
            
            switch (modifierType)
            {
                case ModifierType.Add:
                    context.Magnitude = currentMagnitude + finalValue;
                    break;
                case ModifierType.Multiply:
                    context.Magnitude = currentMagnitude * finalValue;
                    break;
                case ModifierType.Set:
                    context.Magnitude = finalValue;
                    break;
            }
            
            Debug.Log($"Magnitude modified: {currentMagnitude} -> {context.Magnitude}");
        }
    }
} 