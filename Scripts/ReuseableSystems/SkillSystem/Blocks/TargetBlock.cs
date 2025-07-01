using System;
using UnityEngine;

namespace VT.SkillSystem.Blocks
{
    /// <summary>
    /// Defines targeting behavior for the skill
    /// Can only follow Effect blocks
    /// </summary>
    [Serializable]
    public class TargetBlock : SkillBlock
    {
        [Header("Target Settings")]
        [SerializeField] private TargetType targetType = TargetType.Single;
        [SerializeField] private float range = 5f;
        [SerializeField] private LayerMask targetLayers = -1;
        [SerializeField] private bool requireLineOfSight = true;
        
        public enum TargetType
        {
            Self,
            Single,
            Multiple,
            Area,
            Directional
        }
        
        private void OnEnable()
        {
            // Grammar rules: Target can only follow Effect blocks
            validPreviousBlocks.Add(typeof(EffectBlock));
            validNextBlocks.Add(typeof(MagnitudeModifierBlock));
            validNextBlocks.Add(typeof(RangeModifierBlock));
            validNextBlocks.Add(typeof(DurationModifierBlock));
        }
        
        public override void Execute(SkillContext context)
        {
            context.Range = range;
            context.TargetLayer = targetLayers;
            
            switch (targetType)
            {
                case TargetType.Self:
                    TargetSelf(context);
                    break;
                case TargetType.Single:
                    TargetSingle(context);
                    break;
                case TargetType.Multiple:
                    TargetMultiple(context);
                    break;
                case TargetType.Area:
                    TargetArea(context);
                    break;
                case TargetType.Directional:
                    TargetDirectional(context);
                    break;
            }
        }
        
        private void TargetSelf(SkillContext context)
        {
            context.Target = context.Caster;
            context.Position = context.Caster.transform.position;
        }
        
        private void TargetSingle(SkillContext context)
        {
            if (context.Target == null)
            {
                // Find nearest target in range
                var targets = Physics.OverlapSphere(context.Caster.transform.position, range, targetLayers);
                if (targets.Length > 0)
                {
                    context.Target = targets[0].gameObject;
                    context.Position = context.Target.transform.position;
                }
            }
        }
        
        private void TargetMultiple(SkillContext context)
        {
            var targets = Physics.OverlapSphere(context.Caster.transform.position, range, targetLayers);
            context.SetData("targets", targets);
        }
        
        private void TargetArea(SkillContext context)
        {
            context.Position = context.Caster.transform.position + context.Caster.transform.forward * range;
        }
        
        private void TargetDirectional(SkillContext context)
        {
            var direction = context.Caster.transform.forward;
            context.Position = context.Caster.transform.position + direction * range;
            context.SetData("direction", direction);
        }
    }
} 