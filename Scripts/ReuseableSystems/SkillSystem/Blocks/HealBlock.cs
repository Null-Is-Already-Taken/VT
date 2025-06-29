using UnityEngine;
using VT.ReusableSystems.SkillSystem.Core;
using VT.ReusableSystems.SkillSystem.Data;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.SkillSystem.Blocks
{
    /// <summary>
    /// Skill block that heals targets.
    /// </summary>
    public class HealBlock : ISkillBlock
    {
        public string BlockId => "heal";
        public string DisplayName => "Heal Target";
        public string Description => "Heals the target";

        [SerializeField] private float baseHeal = 15f;
        [SerializeField] private float healScaling = 3f;
        [SerializeField] private bool canOverheal = false;
        [SerializeField] private float overhealPercentage = 0.2f;
        [SerializeField] private bool canCrit = true;
        [SerializeField] private float critChance = 0.05f;
        [SerializeField] private float critMultiplier = 1.5f;

        public HealBlock()
        {
            // Default constructor for serialization
        }

        public HealBlock(float baseHeal, float healScaling = 3f)
        {
            this.baseHeal = baseHeal;
            this.healScaling = healScaling;
        }

        public bool CanExecute(SkillExecutionContext context)
        {
            if (context.Target == null)
                return false;

            // Check if target has health component
            var health = context.Target.GetComponent<IHealth>();
            if (health == null)
                return false;

            // Check if target needs healing (unless overheal is allowed)
            if (!canOverheal && health.CurrentHP >= health.MaxHP)
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

            // Calculate heal amount
            float healAmount = CalculateHeal(context);
            
            // Apply healing
            targetHealth.Heal(healAmount);

            // Store heal data in shared data for other blocks to use
            context.SetSharedData("last_heal_amount", healAmount);
            context.SetSharedData("last_heal_target", context.Target);

            // Log healing
            Debug.Log($"[HealBlock] {context.Caster.name} healed {context.Target.name} for {healAmount} HP");

            return SkillBlockResult.Success;
        }

        public ValidationResult Validate()
        {
            if (baseHeal < 0)
                return ValidationResult.Failure("Base heal cannot be negative");

            if (healScaling < 0)
                return ValidationResult.Failure("Heal scaling cannot be negative");

            if (critChance < 0 || critChance > 1)
                return ValidationResult.Failure("Critical chance must be between 0 and 1");

            if (critMultiplier < 1)
                return ValidationResult.Failure("Critical multiplier must be at least 1");

            if (overhealPercentage < 0)
                return ValidationResult.Failure("Overheal percentage cannot be negative");

            return ValidationResult.Success();
        }

        private float CalculateHeal(SkillExecutionContext context)
        {
            // Get skill level
            int skillLevel = context.SkillInstance.Level;
            
            // Calculate base heal with scaling
            float heal = baseHeal + (healScaling * (skillLevel - 1));

            // Apply critical heal
            if (canCrit && context.RandomBool(critChance))
            {
                heal *= critMultiplier;
                context.SetSharedData("last_heal_crit", true);
            }
            else
            {
                context.SetSharedData("last_heal_crit", false);
            }

            // Apply random variation (±5% for healing)
            float variation = context.RandomFloat(0.95f, 1.05f);
            heal *= variation;

            // Round to 1 decimal place
            return Mathf.Round(heal * 10f) / 10f;
        }

        /// <summary>
        /// Configure the heal block with parameters.
        /// </summary>
        public HealBlock Configure(float baseHeal, float healScaling = 3f)
        {
            this.baseHeal = baseHeal;
            this.healScaling = healScaling;
            return this;
        }

        /// <summary>
        /// Configure overheal settings.
        /// </summary>
        public HealBlock ConfigureOverheal(bool canOverheal, float overhealPercentage = 0.2f)
        {
            this.canOverheal = canOverheal;
            this.overhealPercentage = overhealPercentage;
            return this;
        }

        /// <summary>
        /// Configure critical heal settings.
        /// </summary>
        public HealBlock ConfigureCrit(bool canCrit, float critChance = 0.05f, float critMultiplier = 1.5f)
        {
            this.canCrit = canCrit;
            this.critChance = critChance;
            this.critMultiplier = critMultiplier;
            return this;
        }
    }
} 