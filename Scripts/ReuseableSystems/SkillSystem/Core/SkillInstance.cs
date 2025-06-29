using System;
using System.Collections.Generic;
using UnityEngine;
using VT.ReusableSystems.SkillSystem.Data;

namespace VT.ReusableSystems.SkillSystem.Core
{
    /// <summary>
    /// Runtime instance of a skill that manages execution state and cooldowns.
    /// </summary>
    public class SkillInstance
    {
        /// <summary>
        /// The skill definition this instance is based on.
        /// </summary>
        public SkillDefinition SkillDefinition { get; private set; }
        
        /// <summary>
        /// The entity that owns this skill instance.
        /// </summary>
        public GameObject Owner { get; private set; }
        
        /// <summary>
        /// Current cooldown remaining (0 = ready to use).
        /// </summary>
        public float CooldownRemaining { get; private set; }
        
        /// <summary>
        /// Whether the skill is currently on cooldown.
        /// </summary>
        public bool IsOnCooldown => CooldownRemaining > 0;
        
        /// <summary>
        /// Whether the skill is currently being cast.
        /// </summary>
        public bool IsCasting { get; private set; }
        
        /// <summary>
        /// Current cast progress (0-1).
        /// </summary>
        public float CastProgress { get; private set; }
        
        /// <summary>
        /// Time when casting started.
        /// </summary>
        public float CastStartTime { get; private set; }
        
        /// <summary>
        /// Whether the skill is currently active (for toggle/passive skills).
        /// </summary>
        public bool IsActive { get; private set; }
        
        /// <summary>
        /// Current level of the skill.
        /// </summary>
        public int Level { get; private set; }
        
        /// <summary>
        /// Custom data associated with this skill instance.
        /// </summary>
        public Dictionary<string, object> CustomData { get; private set; } = new Dictionary<string, object>();

        // Events
        public event Action<SkillInstance> OnCooldownStarted;
        public event Action<SkillInstance> OnCooldownFinished;
        public event Action<SkillInstance> OnCastStarted;
        public event Action<SkillInstance> OnCastFinished;
        public event Action<SkillInstance> OnCastInterrupted;
        public event Action<SkillInstance> OnActivated;
        public event Action<SkillInstance> OnDeactivated;

        public SkillInstance(SkillDefinition skillDefinition, GameObject owner, int level = 1)
        {
            SkillDefinition = skillDefinition ?? throw new ArgumentNullException(nameof(skillDefinition));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Level = Mathf.Max(1, level);
            
            Reset();
        }

        /// <summary>
        /// Reset the skill instance to its initial state.
        /// </summary>
        public void Reset()
        {
            CooldownRemaining = 0f;
            IsCasting = false;
            CastProgress = 0f;
            CastStartTime = 0f;
            IsActive = false;
            CustomData.Clear();
        }

        /// <summary>
        /// Update the skill instance (call this every frame).
        /// </summary>
        public void Update(float deltaTime)
        {
            // Update cooldown
            if (CooldownRemaining > 0)
            {
                CooldownRemaining -= deltaTime;
                if (CooldownRemaining <= 0)
                {
                    CooldownRemaining = 0f;
                    OnCooldownFinished?.Invoke(this);
                }
            }

            // Update casting
            if (IsCasting)
            {
                float castTime = SkillDefinition.CastTime;
                if (castTime > 0)
                {
                    CastProgress = Mathf.Clamp01((Time.time - CastStartTime) / castTime);
                    if (CastProgress >= 1f)
                    {
                        FinishCasting();
                    }
                }
                else
                {
                    // Instant cast
                    FinishCasting();
                }
            }
        }

        /// <summary>
        /// Start casting the skill.
        /// </summary>
        public bool StartCasting()
        {
            if (!CanCast())
                return false;

            IsCasting = true;
            CastProgress = 0f;
            CastStartTime = Time.time;
            OnCastStarted?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Finish casting the skill.
        /// </summary>
        public void FinishCasting()
        {
            if (!IsCasting)
                return;

            IsCasting = false;
            CastProgress = 1f;
            OnCastFinished?.Invoke(this);
            
            // Start cooldown
            StartCooldown();
        }

        /// <summary>
        /// Interrupt the current cast.
        /// </summary>
        public void InterruptCast()
        {
            if (!IsCasting)
                return;

            IsCasting = false;
            CastProgress = 0f;
            OnCastInterrupted?.Invoke(this);
        }

        /// <summary>
        /// Start the cooldown for this skill.
        /// </summary>
        public void StartCooldown()
        {
            CooldownRemaining = SkillDefinition.Cooldown;
            OnCooldownStarted?.Invoke(this);
        }

        /// <summary>
        /// Activate the skill (for toggle/passive skills).
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            OnActivated?.Invoke(this);
        }

        /// <summary>
        /// Deactivate the skill (for toggle/passive skills).
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            OnDeactivated?.Invoke(this);
        }

        /// <summary>
        /// Check if the skill can be cast.
        /// </summary>
        public bool CanCast()
        {
            if (IsOnCooldown)
                return false;

            if (IsCasting)
                return false;

            if (SkillDefinition.SkillType == SkillType.Passive)
                return false;

            return true;
        }

        /// <summary>
        /// Get the cooldown progress (0-1).
        /// </summary>
        public float GetCooldownProgress()
        {
            if (SkillDefinition.Cooldown <= 0)
                return 1f;

            return Mathf.Clamp01(1f - (CooldownRemaining / SkillDefinition.Cooldown));
        }

        /// <summary>
        /// Get the cast progress (0-1).
        /// </summary>
        public float GetCastProgress()
        {
            return CastProgress;
        }

        /// <summary>
        /// Set custom data for this skill instance.
        /// </summary>
        public void SetCustomData<T>(string key, T value)
        {
            CustomData[key] = value;
        }

        /// <summary>
        /// Get custom data from this skill instance.
        /// </summary>
        public T GetCustomData<T>(string key, T defaultValue = default(T))
        {
            if (CustomData.TryGetValue(key, out object value) && value is T typedValue)
                return typedValue;
            return defaultValue;
        }

        /// <summary>
        /// Check if custom data exists.
        /// </summary>
        public bool HasCustomData(string key)
        {
            return CustomData.ContainsKey(key);
        }

        /// <summary>
        /// Remove custom data.
        /// </summary>
        public void RemoveCustomData(string key)
        {
            CustomData.Remove(key);
        }

        /// <summary>
        /// Clear all custom data.
        /// </summary>
        public void ClearCustomData()
        {
            CustomData.Clear();
        }

        /// <summary>
        /// Get a scaled value based on skill level.
        /// </summary>
        public float GetScaledValue(float baseValue, float scalingPerLevel = 0f)
        {
            return baseValue + (scalingPerLevel * (Level - 1));
        }

        /// <summary>
        /// Get the current mana cost (scaled by level).
        /// </summary>
        public float GetCurrentManaCost()
        {
            return GetScaledValue(SkillDefinition.ManaCost);
        }

        /// <summary>
        /// Get the current range (scaled by level).
        /// </summary>
        public float GetCurrentRange()
        {
            return GetScaledValue(SkillDefinition.Range);
        }

        /// <summary>
        /// Get the current cast time (scaled by level).
        /// </summary>
        public float GetCurrentCastTime()
        {
            return GetScaledValue(SkillDefinition.CastTime);
        }

        /// <summary>
        /// Get the current cooldown (scaled by level).
        /// </summary>
        public float GetCurrentCooldown()
        {
            return GetScaledValue(SkillDefinition.Cooldown);
        }
    }
} 