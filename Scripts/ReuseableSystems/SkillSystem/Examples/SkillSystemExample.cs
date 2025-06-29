using UnityEngine;
using Sirenix.OdinInspector;
using VT.ReusableSystems.SkillSystem.Core;
using VT.ReusableSystems.SkillSystem.Data;
using VT.ReusableSystems.SkillSystem.Assembly;
using VT.ReusableSystems.SkillSystem.Execution;
using VT.ReusableSystems.SkillSystem.Blocks;

namespace VT.ReusableSystems.SkillSystem.Examples
{
    /// <summary>
    /// Example demonstrating how to use the skill system.
    /// Shows skill creation, management, and execution.
    /// </summary>
    public class SkillSystemExample : MonoBehaviour
    {
        [Header("Skill System Components")]
        [SerializeField] private SkillExecutor skillExecutor;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject target;

        [Header("Example Skills")]
        [SerializeField] private SkillDefinition basicAttackSkill;
        [SerializeField] private SkillDefinition fireballSkill;
        [SerializeField] private SkillDefinition healSkill;
        [SerializeField] private SkillDefinition lightningBoltSkill;

        [Header("Runtime Skill Instances")]
        [SerializeField, ReadOnly] private SkillInstance basicAttackInstance;
        [SerializeField, ReadOnly] private SkillInstance fireballInstance;
        [SerializeField, ReadOnly] private SkillInstance healInstance;
        [SerializeField, ReadOnly] private SkillInstance lightningBoltInstance;

        private void Start()
        {
            InitializeSkillSystem();
            CreateExampleSkills();
        }

        private void Update()
        {
            HandleInput();
            UpdateSkillInstances();
        }

        /// <summary>
        /// Initialize the skill system.
        /// </summary>
        private void InitializeSkillSystem()
        {
            // Get or create skill executor
            if (skillExecutor == null)
            {
                skillExecutor = FindObjectOfType<SkillExecutor>();
                if (skillExecutor == null)
                {
                    var executorGO = new GameObject("SkillExecutor");
                    skillExecutor = executorGO.AddComponent<SkillExecutor>();
                }
            }

            // Set up event bus (using the existing event system)
            skillExecutor.SetEventBus(new SkillEventBus());

            // Subscribe to skill events
            skillExecutor.OnSkillStarted += OnSkillStarted;
            skillExecutor.OnSkillCompleted += OnSkillCompleted;
            skillExecutor.OnSkillFailed += OnSkillFailed;
            skillExecutor.OnSkillInterrupted += OnSkillInterrupted;
        }

        /// <summary>
        /// Create example skills using the fluent API.
        /// </summary>
        private void CreateExampleSkills()
        {
            // Create basic attack skill
            basicAttackSkill = SkillBuilder.CreateDamageSkill("basic_attack", "Basic Attack", 15f, 0.5f);
            basicAttackInstance = new SkillInstance(basicAttackSkill, player, 1);

            // Create fireball skill
            fireballSkill = new SkillBuilder("fireball", "Fireball")
                .WithDescription("Launches a fireball that deals fire damage")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Direction)
                .WithCooldown(2f)
                .WithRange(15f)
                .WithManaCost(25f)
                .AddDamage(30f, 5f, DamageType.Fire)
                .AddVisualEffect(null) // Would be a fireball prefab
                .Build();
            fireballInstance = new SkillInstance(fireballSkill, player, 1);

            // Create heal skill
            healSkill = SkillBuilder.CreateHealSkill("heal", "Heal", 25f, 3f);
            healInstance = new SkillInstance(healSkill, player, 1);

            // Create lightning bolt skill
            lightningBoltSkill = new SkillBuilder("lightning_bolt", "Lightning Bolt")
                .WithDescription("Lightning bolt that deals lightning damage and chains to nearby targets")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Single)
                .WithCooldown(4f)
                .WithRange(8f)
                .WithManaCost(30f)
                .AddDamage(40f, 6f, DamageType.Lightning)
                .AddChainBlock(new DamageBlock(28f, 4f, DamageType.Lightning))
                .AddVisualEffect(null) // Lightning effect
                .Build();
            lightningBoltInstance = new SkillInstance(lightningBoltSkill, player, 1);

            Debug.Log("Example skills created successfully!");
        }

        /// <summary>
        /// Handle input for skill execution.
        /// </summary>
        private void HandleInput()
        {
            if (skillExecutor == null) return;

            // Basic attack (left click)
            if (Input.GetMouseButtonDown(0))
            {
                ExecuteSkill(basicAttackInstance, target);
            }

            // Fireball (right click)
            if (Input.GetMouseButtonDown(1))
            {
                ExecuteSkill(fireballInstance, target);
            }

            // Heal (H key)
            if (Input.GetKeyDown(KeyCode.H))
            {
                ExecuteSkill(healInstance, player); // Self-heal
            }

            // Lightning bolt (L key)
            if (Input.GetKeyDown(KeyCode.L))
            {
                ExecuteSkill(lightningBoltInstance, target);
            }

            // Interrupt current skill (I key)
            if (Input.GetKeyDown(KeyCode.I))
            {
                skillExecutor.StopAllSkills();
            }
        }

        /// <summary>
        /// Execute a skill.
        /// </summary>
        private void ExecuteSkill(SkillInstance skillInstance, GameObject target)
        {
            if (skillInstance == null || skillExecutor == null) return;

            // Create input data
            var inputData = new SkillInputData
            {
                MousePosition = Input.mousePosition,
                MouseWorldPosition = GetMouseWorldPosition(),
                Direction = (target.transform.position - player.transform.position).normalized
            };

            // Execute the skill
            bool success = skillExecutor.ExecuteSkill(skillInstance, target, inputData);
            
            if (success)
            {
                Debug.Log($"Executing skill: {skillInstance.SkillDefinition.DisplayName}");
            }
            else
            {
                Debug.LogWarning($"Failed to execute skill: {skillInstance.SkillDefinition.DisplayName}");
            }
        }

        /// <summary>
        /// Get mouse position in world space.
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            if (Camera.main == null) return Vector3.zero;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Distance from camera
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// Update skill instances (cooldowns, etc.).
        /// </summary>
        private void UpdateSkillInstances()
        {
            basicAttackInstance?.Update(Time.deltaTime);
            fireballInstance?.Update(Time.deltaTime);
            healInstance?.Update(Time.deltaTime);
            lightningBoltInstance?.Update(Time.deltaTime);
        }

        // Skill event handlers
        private void OnSkillStarted(SkillInstance skillInstance)
        {
            Debug.Log($"Skill started: {skillInstance.SkillDefinition.DisplayName}");
        }

        private void OnSkillCompleted(SkillInstance skillInstance)
        {
            Debug.Log($"Skill completed: {skillInstance.SkillDefinition.DisplayName}");
        }

        private void OnSkillFailed(SkillInstance skillInstance)
        {
            Debug.LogWarning($"Skill failed: {skillInstance.SkillDefinition.DisplayName}");
        }

        private void OnSkillInterrupted(SkillInstance skillInstance)
        {
            Debug.Log($"Skill interrupted: {skillInstance.SkillDefinition.DisplayName}");
        }

        // Inspector buttons for testing
        [Button("Test Basic Attack")]
        private void TestBasicAttack()
        {
            ExecuteSkill(basicAttackInstance, target);
        }

        [Button("Test Fireball")]
        private void TestFireball()
        {
            ExecuteSkill(fireballInstance, target);
        }

        [Button("Test Heal")]
        private void TestHeal()
        {
            ExecuteSkill(healInstance, player);
        }

        [Button("Test Lightning Bolt")]
        private void TestLightningBolt()
        {
            ExecuteSkill(lightningBoltInstance, target);
        }

        [Button("Stop All Skills")]
        private void StopAllSkills()
        {
            skillExecutor?.StopAllSkills();
        }

        [Button("Create Complex Skill")]
        private void CreateComplexSkill()
        {
            // Create a complex skill with multiple effects
            var complexSkill = new SkillBuilder("complex_skill", "Complex Skill")
                .WithDescription("A complex skill with multiple effects")
                .WithType(SkillType.Active)
                .WithTargetType(TargetType.Area)
                .WithCooldown(8f)
                .WithCastTime(1.5f)
                .WithRange(10f)
                .WithManaCost(50f)
                .AddDamage(50f, 8f, DamageType.Fire)
                .AddKnockback(5f, 2f)
                .AddStun(2f)
                .AddVisualEffect(null) // Explosion effect
                .AddAudioEffect(null, 0.8f) // Explosion sound
                .Build();

            var complexInstance = new SkillInstance(complexSkill, player, 1);
            ExecuteSkill(complexInstance, target);

            Debug.Log("Complex skill created and executed!");
        }

        [Button("Level Up All Skills")]
        private void LevelUpAllSkills()
        {
            basicAttackInstance = new SkillInstance(basicAttackSkill, player, basicAttackInstance.Level + 1);
            fireballInstance = new SkillInstance(fireballSkill, player, fireballInstance.Level + 1);
            healInstance = new SkillInstance(healSkill, player, healInstance.Level + 1);
            lightningBoltInstance = new SkillInstance(lightningBoltSkill, player, lightningBoltInstance.Level + 1);

            Debug.Log("All skills leveled up!");
        }
    }

    /// <summary>
    /// Simple event bus implementation for skill events.
    /// </summary>
    public class SkillEventBus : IEventBus
    {
        public void Raise<T>(T eventData) where T : IEvent
        {
            // This would integrate with the existing event system
            Debug.Log($"Skill event raised: {typeof(T).Name}");
        }
    }
} 