using System;
using System.Collections.Generic;
using UnityEngine;
using VT.ReusableSystems.SkillSystem.Core;
using VT.ReusableSystems.SkillSystem.Data;
using VT.ReusableSystems.SkillSystem.Blocks;

namespace VT.ReusableSystems.SkillSystem.Assembly
{
    /// <summary>
    /// Fluent API for building skills by composing skill blocks.
    /// This provides a clean, readable way to create complex skills from simple blocks.
    /// </summary>
    public class SkillBuilder
    {
        private SkillDefinition skillDefinition;
        private List<SkillBlockData> skillBlocks = new List<SkillBlockData>();
        private Dictionary<string, ISkillBlock> blockRegistry = new Dictionary<string, ISkillBlock>();

        public SkillBuilder(string skillId, string displayName)
        {
            skillDefinition = ScriptableObject.CreateInstance<SkillDefinition>();
            skillDefinition.name = displayName;
            
            // Set basic properties
            SetProperty("skillId", skillId);
            SetProperty("displayName", displayName);
        }

        /// <summary>
        /// Set a property on the skill definition using reflection.
        /// </summary>
        private void SetProperty(string propertyName, object value)
        {
            var property = typeof(SkillDefinition).GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(skillDefinition, value);
            }
        }

        /// <summary>
        /// Set the skill description.
        /// </summary>
        public SkillBuilder WithDescription(string description)
        {
            SetProperty("description", description);
            return this;
        }

        /// <summary>
        /// Set the skill icon.
        /// </summary>
        public SkillBuilder WithIcon(Sprite icon)
        {
            SetProperty("icon", icon);
            return this;
        }

        /// <summary>
        /// Set the skill type.
        /// </summary>
        public SkillBuilder WithType(SkillType skillType)
        {
            SetProperty("skillType", skillType);
            return this;
        }

        /// <summary>
        /// Set the target type.
        /// </summary>
        public SkillBuilder WithTargetType(TargetType targetType)
        {
            SetProperty("targetType", targetType);
            return this;
        }

        /// <summary>
        /// Set the cooldown.
        /// </summary>
        public SkillBuilder WithCooldown(float cooldown)
        {
            SetProperty("cooldown", cooldown);
            return this;
        }

        /// <summary>
        /// Set the cast time.
        /// </summary>
        public SkillBuilder WithCastTime(float castTime)
        {
            SetProperty("castTime", castTime);
            return this;
        }

        /// <summary>
        /// Set the range.
        /// </summary>
        public SkillBuilder WithRange(float range)
        {
            SetProperty("range", range);
            return this;
        }

        /// <summary>
        /// Set the mana cost.
        /// </summary>
        public SkillBuilder WithManaCost(float manaCost)
        {
            SetProperty("manaCost", manaCost);
            return this;
        }

        /// <summary>
        /// Set whether the skill requires a target.
        /// </summary>
        public SkillBuilder RequiresTarget(bool requiresTarget)
        {
            SetProperty("requiresTarget", requiresTarget);
            return this;
        }

        /// <summary>
        /// Set whether the skill can be interrupted.
        /// </summary>
        public SkillBuilder CanBeInterrupted(bool canBeInterrupted)
        {
            SetProperty("canBeInterrupted", canBeInterrupted);
            return this;
        }

        /// <summary>
        /// Set the required level.
        /// </summary>
        public SkillBuilder WithRequiredLevel(int requiredLevel)
        {
            SetProperty("requiredLevel", requiredLevel);
            return this;
        }

        /// <summary>
        /// Add a damage block to the skill.
        /// </summary>
        public SkillBuilder AddDamage(float baseDamage, float damageScaling = 5f, DamageType damageType = DamageType.Physical)
        {
            var damageBlock = new DamageBlock(baseDamage, damageScaling, damageType);
            return AddBlock(damageBlock, "Deal Damage");
        }

        /// <summary>
        /// Add a heal block to the skill.
        /// </summary>
        public SkillBuilder AddHeal(float baseHeal, float healScaling = 3f)
        {
            var healBlock = new HealBlock(baseHeal, healScaling);
            return AddBlock(healBlock, "Heal Target");
        }

        /// <summary>
        /// Add a custom block to the skill.
        /// </summary>
        public SkillBuilder AddBlock(ISkillBlock block, string displayName = null, int executionOrder = 0, float delay = 0f)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var blockData = new SkillBlockData
            {
                BlockId = block.BlockId,
                DisplayName = displayName ?? block.DisplayName,
                ExecutionOrder = executionOrder,
                Delay = delay
            };

            skillBlocks.Add(blockData);
            blockRegistry[block.BlockId] = block;

            return this;
        }

        /// <summary>
        /// Add a conditional block that only executes if the condition is met.
        /// </summary>
        public SkillBuilder AddConditionalBlock(ISkillBlock block, Func<SkillExecutionContext, bool> condition, string displayName = null)
        {
            var conditionalBlock = new ConditionalBlock(block, condition);
            return AddBlock(conditionalBlock, displayName ?? $"Conditional {block.DisplayName}");
        }

        /// <summary>
        /// Add a delayed block that executes after a delay.
        /// </summary>
        public SkillBuilder AddDelayedBlock(ISkillBlock block, float delay, string displayName = null)
        {
            return AddBlock(block, displayName ?? $"Delayed {block.DisplayName}", 0, delay);
        }

        /// <summary>
        /// Add a repeated block that executes multiple times.
        /// </summary>
        public SkillBuilder AddRepeatedBlock(ISkillBlock block, int repeatCount, float interval = 0.5f, string displayName = null)
        {
            var repeatedBlock = new RepeatedBlock(block, repeatCount, interval);
            return AddBlock(repeatedBlock, displayName ?? $"Repeated {block.DisplayName}");
        }

        /// <summary>
        /// Add a chain block that executes if the previous block succeeded.
        /// </summary>
        public SkillBuilder AddChainBlock(ISkillBlock block, string displayName = null)
        {
            var chainBlock = new ChainBlock(block);
            return AddBlock(chainBlock, displayName ?? $"Chain {block.DisplayName}");
        }

        /// <summary>
        /// Add a visual effect block.
        /// </summary>
        public SkillBuilder AddVisualEffect(GameObject effectPrefab, Vector3 offset = default, bool followTarget = false)
        {
            var effectBlock = new VisualEffectBlock(effectPrefab, offset, followTarget);
            return AddBlock(effectBlock, "Visual Effect");
        }

        /// <summary>
        /// Add an audio effect block.
        /// </summary>
        public SkillBuilder AddAudioEffect(AudioClip audioClip, float volume = 1f, bool followTarget = false)
        {
            var audioBlock = new AudioEffectBlock(audioClip, volume, followTarget);
            return AddBlock(audioBlock, "Audio Effect");
        }

        /// <summary>
        /// Add a movement block.
        /// </summary>
        public SkillBuilder AddMovement(float distance, float duration = 1f, bool towardsTarget = false)
        {
            var movementBlock = new MovementBlock(distance, duration, towardsTarget);
            return AddBlock(movementBlock, "Movement");
        }

        /// <summary>
        /// Add a knockback block.
        /// </summary>
        public SkillBuilder AddKnockback(float force, float upwardForce = 0f)
        {
            var knockbackBlock = new KnockbackBlock(force, upwardForce);
            return AddBlock(knockbackBlock, "Knockback");
        }

        /// <summary>
        /// Add a stun block.
        /// </summary>
        public SkillBuilder AddStun(float duration)
        {
            var stunBlock = new StunBlock(duration);
            return AddBlock(stunBlock, "Stun");
        }

        /// <summary>
        /// Add a buff block.
        /// </summary>
        public SkillBuilder AddBuff(string buffId, float duration, float magnitude = 1f)
        {
            var buffBlock = new BuffBlock(buffId, duration, magnitude);
            return AddBlock(buffBlock, "Apply Buff");
        }

        /// <summary>
        /// Add a debuff block.
        /// </summary>
        public SkillBuilder AddDebuff(string debuffId, float duration, float magnitude = 1f)
        {
            var debuffBlock = new DebuffBlock(debuffId, duration, magnitude);
            return AddBlock(debuffBlock, "Apply Debuff");
        }

        /// <summary>
        /// Add a teleport block.
        /// </summary>
        public SkillBuilder AddTeleport(float maxDistance = 10f, bool towardsTarget = false)
        {
            var teleportBlock = new TeleportBlock(maxDistance, towardsTarget);
            return AddBlock(teleportBlock, "Teleport");
        }

        /// <summary>
        /// Add a projectile block.
        /// </summary>
        public SkillBuilder AddProjectile(GameObject projectilePrefab, float speed = 10f, float lifetime = 5f)
        {
            var projectileBlock = new ProjectileBlock(projectilePrefab, speed, lifetime);
            return AddBlock(projectileBlock, "Launch Projectile");
        }

        /// <summary>
        /// Add an area of effect block.
        /// </summary>
        public SkillBuilder AddAreaOfEffect(float radius, LayerMask targetLayers)
        {
            var aoeBlock = new AreaOfEffectBlock(radius, targetLayers);
            return AddBlock(aoeBlock, "Area of Effect");
        }

        /// <summary>
        /// Build the final skill definition.
        /// </summary>
        public SkillDefinition Build()
        {
            // Sort blocks by execution order
            skillBlocks.Sort((a, b) => a.ExecutionOrder.CompareTo(b.ExecutionOrder));

            // Add blocks to skill definition
            foreach (var blockData in skillBlocks)
            {
                skillDefinition.AddSkillBlock(blockData);
            }

            // Validate the skill
            var validation = skillDefinition.Validate();
            if (!validation.IsValid)
            {
                Debug.LogError($"Skill validation failed: {validation.ErrorMessage}");
            }
            else if (!string.IsNullOrEmpty(validation.WarningMessage))
            {
                Debug.LogWarning($"Skill validation warning: {validation.WarningMessage}");
            }

            return skillDefinition;
        }

        /// <summary>
        /// Create a simple damage skill.
        /// </summary>
        public static SkillDefinition CreateDamageSkill(string skillId, string displayName, float damage, float cooldown = 1f)
        {
            return new SkillBuilder(skillId, displayName)
                .WithDescription($"Deals {damage} damage to the target")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Single)
                .WithCooldown(cooldown)
                .WithRange(5f)
                .WithManaCost(10f)
                .AddDamage(damage)
                .Build();
        }

        /// <summary>
        /// Create a simple heal skill.
        /// </summary>
        public static SkillDefinition CreateHealSkill(string skillId, string displayName, float heal, float cooldown = 3f)
        {
            return new SkillBuilder(skillId, displayName)
                .WithDescription($"Heals the target for {heal} HP")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Single)
                .WithCooldown(cooldown)
                .WithRange(5f)
                .WithManaCost(20f)
                .AddHeal(heal)
                .Build();
        }

        /// <summary>
        /// Create a fireball skill.
        /// </summary>
        public static SkillDefinition CreateFireballSkill(string skillId, string displayName, float damage, GameObject projectilePrefab)
        {
            return new SkillBuilder(skillId, displayName)
                .WithDescription($"Launches a fireball that deals {damage} fire damage")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Direction)
                .WithCooldown(2f)
                .WithRange(15f)
                .WithManaCost(25f)
                .AddProjectile(projectilePrefab, 15f, 3f)
                .AddDamage(damage, 3f, DamageType.Fire)
                .AddVisualEffect(projectilePrefab)
                .Build();
        }

        /// <summary>
        /// Create a lightning bolt skill.
        /// </summary>
        public static SkillDefinition CreateLightningBoltSkill(string skillId, string displayName, float damage, float chainCount = 3)
        {
            return new SkillBuilder(skillId, displayName)
                .WithDescription($"Lightning bolt that deals {damage} lightning damage and chains to {chainCount} targets")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Single)
                .WithCooldown(4f)
                .WithRange(8f)
                .WithManaCost(30f)
                .AddDamage(damage, 4f, DamageType.Lightning)
                .AddChainBlock(new DamageBlock(damage * 0.7f, 3f, DamageType.Lightning))
                .AddVisualEffect(null) // Lightning effect
                .Build();
        }
    }
} 