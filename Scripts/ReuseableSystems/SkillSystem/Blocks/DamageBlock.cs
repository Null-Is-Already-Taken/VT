using UnityEngine;
using VT.ReusableSystems.SkillSystem.Core;
using VT.ReusableSystems.SkillSystem.Data;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.SkillSystem.Blocks
{
    /// <summary>
    /// Skill block that deals damage to targets.
    /// </summary>
    public class DamageBlock : ISkillBlock
    {
        public string BlockId => "damage";
        public string DisplayName => "Deal Damage";
        public string Description => "Deals damage to the target";

        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float damageScaling = 5f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        [SerializeField] private bool canCrit = true;
        [SerializeField] private float critChance = 0.1f;
        [SerializeField] private float critMultiplier = 2f;
        [SerializeField] private bool ignoreArmor = false;

        public DamageBlock()
        {
            // Default constructor for serialization
        }

        public DamageBlock(float baseDamage, float damageScaling = 5f, DamageType damageType = DamageType.Physical)
        {
            this.baseDamage = baseDamage;
            this.damageScaling = damageScaling;
            this.damageType = damageType;
        }

        public bool CanExecute(SkillExecutionContext context)
        {
            if (context.Target == null)
                return false;

            // Check if target has health component
            var health = context.Target.GetComponent<IHealth>();
            if (health == null)
                return false;

            // Check if target is alive
            if (health.IsDead)
                return false;

            return true;
        }

        public SkillBlockResult Execute(SkillExecutionContext context)
        {
            if (!CanExecute(context))
                return SkillBlockResult.Failure;

            var targetHealth = context.Target.GetComponent<IHealth>();
            if (targetHealth == null)
                return SkillBlockResult.Failure;

            // Calculate damage
            float damage = CalculateDamage(context);
            
            // Apply damage
            targetHealth.TakeDamage(damage);

            // Store damage in shared data for other blocks to use
            context.SetSharedData("last_damage_dealt", damage);
            context.SetSharedData("last_damage_target", context.Target);

            // Log damage dealt
            Debug.Log($"[DamageBlock] {context.Caster.name} dealt {damage} damage to {context.Target.name}");

            return SkillBlockResult.Success;
        }

        public ValidationResult Validate()
        {
            if (baseDamage < 0)
                return ValidationResult.Failure("Base damage cannot be negative");

            if (damageScaling < 0)
                return ValidationResult.Failure("Damage scaling cannot be negative");

            if (critChance < 0 || critChance > 1)
                return ValidationResult.Failure("Critical chance must be between 0 and 1");

            if (critMultiplier < 1)
                return ValidationResult.Failure("Critical multiplier must be at least 1");

            return ValidationResult.Success();
        }

        private float CalculateDamage(SkillExecutionContext context)
        {
            // Get skill level
            int skillLevel = context.SkillInstance.Level;
            
            // Calculate base damage with scaling
            float damage = baseDamage + (damageScaling * (skillLevel - 1));

            // Apply critical hit
            if (canCrit && context.RandomBool(critChance))
            {
                damage *= critMultiplier;
                context.SetSharedData("last_crit", true);
            }
            else
            {
                context.SetSharedData("last_crit", false);
            }

            // Apply random variation (±10%)
            float variation = context.RandomFloat(0.9f, 1.1f);
            damage *= variation;

            // Round to 1 decimal place
            return Mathf.Round(damage * 10f) / 10f;
        }

        /// <summary>
        /// Configure the damage block with parameters.
        /// </summary>
        public DamageBlock Configure(float baseDamage, float damageScaling = 5f, DamageType damageType = DamageType.Physical)
        {
            this.baseDamage = baseDamage;
            this.damageScaling = damageScaling;
            this.damageType = damageType;
            return this;
        }

        /// <summary>
        /// Configure critical hit settings.
        /// </summary>
        public DamageBlock ConfigureCrit(bool canCrit, float critChance = 0.1f, float critMultiplier = 2f)
        {
            this.canCrit = canCrit;
            this.critChance = critChance;
            this.critMultiplier = critMultiplier;
            return this;
        }

        /// <summary>
        /// Set whether this damage ignores armor.
        /// </summary>
        public DamageBlock SetIgnoreArmor(bool ignoreArmor)
        {
            this.ignoreArmor = ignoreArmor;
            return this;
        }
    }

    /// <summary>
    /// Types of damage that can be dealt.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Lightning,
        Poison,
        True
    }
} 