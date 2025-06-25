using System.Collections.Generic;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions
{
    [CreateAssetMenu(menuName = "Procedural Effect Builder/Skill Template")]
    public class SkillTemplate : ScriptableObject
    {
        [Header("Core Grammar")]
        public SourceType source;
        public TargetType target;
        public EffectDefinition effect;

        [Header("Optional Modifiers")]
        public MagnitudeDefinition magnitude;
        public TimingModifier timing;
        public ConditionDefinition condition;

        [Header("Visuals (Cosmetic Only)")]
        public List<FeedbackDecorator> feedbackDecorators;

        [Header("Advanced")]
        public EffectComposition compositionOverride;
    }
}
