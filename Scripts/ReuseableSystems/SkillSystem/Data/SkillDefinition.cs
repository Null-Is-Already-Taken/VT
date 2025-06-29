using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace VT.ReusableSystems.SkillSystem.Data
{
    /// <summary>
    /// Data-oriented skill definition using ScriptableObject.
    /// Contains all the configuration data for a skill without any logic.
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "VT/Skill System/Skill Definition")]
    public class SkillDefinition : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] private string skillId = "skill_id";
        [SerializeField] private string displayName = "Skill Name";
        [SerializeField, TextArea(3, 5)] private string description = "Skill description";
        [SerializeField] private Sprite icon;
        
        [Header("Skill Configuration")]
        [SerializeField] private SkillType skillType = SkillType.Active;
        [SerializeField] private TargetType targetType = TargetType.Single;
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private float castTime = 0f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float manaCost = 10f;
        [SerializeField] private bool requiresTarget = true;
        [SerializeField] private bool canBeInterrupted = true;
        
        [Header("Skill Blocks")]
        [SerializeField, ListDrawerSettings(ShowIndexLabels = true)] 
        private List<SkillBlockData> skillBlocks = new List<SkillBlockData>();
        
        [Header("Requirements")]
        [SerializeField] private int requiredLevel = 1;
        [SerializeField] private List<SkillRequirement> requirements = new List<SkillRequirement>();
        
        [Header("Visual & Audio")]
        [SerializeField] private GameObject castEffect;
        [SerializeField] private AudioClip castSound;
        [SerializeField] private GameObject impactEffect;
        [SerializeField] private AudioClip impactSound;

        // Properties
        public string SkillId => skillId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public SkillType SkillType => skillType;
        public TargetType TargetType => targetType;
        public float Cooldown => cooldown;
        public float CastTime => castTime;
        public float Range => range;
        public float ManaCost => manaCost;
        public bool RequiresTarget => requiresTarget;
        public bool CanBeInterrupted => canBeInterrupted;
        public IReadOnlyList<SkillBlockData> SkillBlocks => skillBlocks;
        public int RequiredLevel => requiredLevel;
        public IReadOnlyList<SkillRequirement> Requirements => requirements;
        public GameObject CastEffect => castEffect;
        public AudioClip CastSound => castSound;
        public GameObject ImpactEffect => impactEffect;
        public AudioClip ImpactSound => impactSound;

        /// <summary>
        /// Validate the skill definition.
        /// </summary>
        public ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(skillId))
                return ValidationResult.Failure("Skill ID cannot be empty");
                
            if (string.IsNullOrEmpty(displayName))
                return ValidationResult.Failure("Display name cannot be empty");
                
            if (cooldown < 0)
                return ValidationResult.Failure("Cooldown cannot be negative");
                
            if (castTime < 0)
                return ValidationResult.Failure("Cast time cannot be negative");
                
            if (range < 0)
                return ValidationResult.Failure("Range cannot be negative");
                
            if (manaCost < 0)
                return ValidationResult.Failure("Mana cost cannot be negative");
                
            if (skillBlocks.Count == 0)
                return ValidationResult.Warning("Skill has no blocks defined");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Get a skill block data by index.
        /// </summary>
        public SkillBlockData GetSkillBlock(int index)
        {
            if (index >= 0 && index < skillBlocks.Count)
                return skillBlocks[index];
            return null;
        }

        /// <summary>
        /// Add a skill block to this skill.
        /// </summary>
        public void AddSkillBlock(SkillBlockData blockData)
        {
            if (blockData != null)
                skillBlocks.Add(blockData);
        }

        /// <summary>
        /// Remove a skill block from this skill.
        /// </summary>
        public void RemoveSkillBlock(int index)
        {
            if (index >= 0 && index < skillBlocks.Count)
                skillBlocks.RemoveAt(index);
        }

        /// <summary>
        /// Clear all skill blocks.
        /// </summary>
        public void ClearSkillBlocks()
        {
            skillBlocks.Clear();
        }
    }

    /// <summary>
    /// Type of skill.
    /// </summary>
    public enum SkillType
    {
        Active,     // Player-activated skill
        Passive,    // Always active skill
        Toggle,     // Can be turned on/off
        Ultimate    // Special powerful skill
    }

    /// <summary>
    /// Type of targeting for the skill.
    /// </summary>
    public enum TargetType
    {
        None,       // No target needed
        Self,       // Target is the caster
        Single,     // Single target
        Multiple,   // Multiple targets
        Area,       // Area of effect
        Direction   // Directional skill
    }

    /// <summary>
    /// Data for a single skill block within a skill.
    /// </summary>
    [Serializable]
    public class SkillBlockData
    {
        [SerializeField] private string blockId = "";
        [SerializeField] private string displayName = "Block";
        [SerializeField] private int executionOrder = 0;
        [SerializeField] private bool isOptional = false;
        [SerializeField] private float delay = 0f;
        [SerializeField] private List<SkillBlockParameter> parameters = new List<SkillBlockParameter>();

        public string BlockId => blockId;
        public string DisplayName => displayName;
        public int ExecutionOrder => executionOrder;
        public bool IsOptional => isOptional;
        public float Delay => delay;
        public IReadOnlyList<SkillBlockParameter> Parameters => parameters;

        /// <summary>
        /// Get a parameter by name.
        /// </summary>
        public SkillBlockParameter GetParameter(string name)
        {
            return parameters.Find(p => p.Name == name);
        }

        /// <summary>
        /// Get a parameter value of a specific type.
        /// </summary>
        public T GetParameterValue<T>(string name, T defaultValue = default(T))
        {
            var param = GetParameter(name);
            if (param != null && param.Value is T typedValue)
                return typedValue;
            return defaultValue;
        }

        /// <summary>
        /// Add a parameter to this block.
        /// </summary>
        public void AddParameter(string name, object value)
        {
            parameters.Add(new SkillBlockParameter { Name = name, Value = value });
        }
    }

    /// <summary>
    /// Parameter for a skill block.
    /// </summary>
    [Serializable]
    public class SkillBlockParameter
    {
        [SerializeField] private string name = "";
        [SerializeField] private object value;

        public string Name => name;
        public object Value => value;

        public SkillBlockParameter() { }

        public SkillBlockParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }

    /// <summary>
    /// Requirement for learning or using a skill.
    /// </summary>
    [Serializable]
    public class SkillRequirement
    {
        [SerializeField] private RequirementType type = RequirementType.Level;
        [SerializeField] private string requirementId = "";
        [SerializeField] private float value = 1f;

        public RequirementType Type => type;
        public string RequirementId => requirementId;
        public float Value => value;

        public enum RequirementType
        {
            Level,
            Skill,
            Item,
            Attribute,
            Quest,
            Custom
        }
    }
} 