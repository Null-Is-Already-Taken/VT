using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT.ReusableSystems.SkillSystem.Core;
using VT.ReusableSystems.SkillSystem.Data;
using VT.ReusableSystems.SkillSystem.Blocks;

namespace VT.ReusableSystems.SkillSystem.Execution
{
    /// <summary>
    /// Manages the execution of skills at runtime.
    /// Handles skill casting, block execution, and state management.
    /// </summary>
    public class SkillExecutor : MonoBehaviour
    {
        [Header("Skill Executor Settings")]
        [SerializeField] private bool enableDebugLogging = true;
        [SerializeField] private float maxExecutionTime = 10f; // Prevent infinite loops

        // Block registry for creating block instances
        private Dictionary<string, Type> blockRegistry = new Dictionary<string, Type>();
        
        // Currently executing skills
        private List<SkillExecution> activeExecutions = new List<SkillExecution>();
        
        // Event bus for skill events
        private IEventBus eventBus;

        // Events
        public event Action<SkillInstance> OnSkillStarted;
        public event Action<SkillInstance> OnSkillCompleted;
        public event Action<SkillInstance> OnSkillFailed;
        public event Action<SkillInstance> OnSkillInterrupted;

        private void Awake()
        {
            InitializeBlockRegistry();
        }

        private void Update()
        {
            UpdateActiveExecutions();
        }

        /// <summary>
        /// Initialize the block registry with all available skill blocks.
        /// </summary>
        private void InitializeBlockRegistry()
        {
            // Register built-in blocks
            RegisterBlock<DamageBlock>("damage");
            RegisterBlock<HealBlock>("heal");
            
            // Register other blocks as they're created
            // RegisterBlock<VisualEffectBlock>("visual_effect");
            // RegisterBlock<AudioEffectBlock>("audio_effect");
            // RegisterBlock<MovementBlock>("movement");
            // etc.
        }

        /// <summary>
        /// Register a skill block type.
        /// </summary>
        public void RegisterBlock<T>(string blockId) where T : ISkillBlock
        {
            blockRegistry[blockId] = typeof(T);
        }

        /// <summary>
        /// Execute a skill.
        /// </summary>
        public bool ExecuteSkill(SkillInstance skillInstance, GameObject target = null, SkillInputData inputData = null)
        {
            if (skillInstance == null)
            {
                LogError("Cannot execute null skill instance");
                return false;
            }

            if (!skillInstance.CanCast())
            {
                LogWarning($"Skill {skillInstance.SkillDefinition.DisplayName} cannot be cast");
                return false;
            }

            // Create execution context
            var context = CreateExecutionContext(skillInstance, target, inputData);

            // Start casting
            if (skillInstance.SkillDefinition.CastTime > 0)
            {
                skillInstance.StartCasting();
                LogInfo($"Started casting {skillInstance.SkillDefinition.DisplayName}");
            }

            // Create execution
            var execution = new SkillExecution(skillInstance, context);
            activeExecutions.Add(execution);

            // Execute immediately if no cast time
            if (skillInstance.SkillDefinition.CastTime <= 0)
            {
                ExecuteSkillBlocks(execution);
            }

            OnSkillStarted?.Invoke(skillInstance);
            return true;
        }

        /// <summary>
        /// Create execution context for a skill.
        /// </summary>
        private SkillExecutionContext CreateExecutionContext(SkillInstance skillInstance, GameObject target, SkillInputData inputData)
        {
            return new SkillExecutionContext
            {
                Caster = skillInstance.Owner,
                Target = target,
                Skill = skillInstance.SkillDefinition,
                SkillInstance = skillInstance,
                ExecutionTime = Time.time,
                DeltaTime = Time.deltaTime,
                InputData = inputData ?? new SkillInputData(),
                EventBus = eventBus,
                Camera = Camera.main,
                PhysicsLayerMask = LayerMask.GetMask("Default"),
                Random = new System.Random(UnityEngine.Random.Range(0, int.MaxValue))
            };
        }

        /// <summary>
        /// Update all active skill executions.
        /// </summary>
        private void UpdateActiveExecutions()
        {
            for (int i = activeExecutions.Count - 1; i >= 0; i--)
            {
                var execution = activeExecutions[i];
                
                // Check for timeout
                if (Time.time - execution.StartTime > maxExecutionTime)
                {
                    LogError($"Skill execution timed out: {execution.SkillInstance.SkillDefinition.DisplayName}");
                    InterruptExecution(execution);
                    continue;
                }

                // Update execution
                execution.Update(Time.deltaTime);

                // Remove completed executions
                if (execution.IsCompleted)
                {
                    activeExecutions.RemoveAt(i);
                    OnSkillCompleted?.Invoke(execution.SkillInstance);
                }
            }
        }

        /// <summary>
        /// Execute all blocks in a skill.
        /// </summary>
        private void ExecuteSkillBlocks(SkillExecution execution)
        {
            var skillDefinition = execution.SkillInstance.SkillDefinition;
            var context = execution.Context;

            LogInfo($"Executing skill: {skillDefinition.DisplayName}");

            // Sort blocks by execution order
            var sortedBlocks = skillDefinition.SkillBlocks.OrderBy(b => b.ExecutionOrder).ToList();

            foreach (var blockData in sortedBlocks)
            {
                // Check if execution was interrupted
                if (execution.IsInterrupted)
                    break;

                // Create block instance
                var block = CreateBlockInstance(blockData);
                if (block == null)
                {
                    LogError($"Failed to create block instance: {blockData.BlockId}");
                    continue;
                }

                // Execute block
                var result = ExecuteBlock(block, context, blockData);
                
                // Handle block result
                if (result == SkillBlockResult.Failure)
                {
                    LogWarning($"Block {blockData.DisplayName} failed");
                    if (!blockData.IsOptional)
                    {
                        execution.Fail();
                        break;
                    }
                }
                else if (result == SkillBlockResult.Interrupted)
                {
                    execution.Interrupt();
                    break;
                }
            }

            // Mark execution as completed
            execution.Complete();
        }

        /// <summary>
        /// Create a block instance from block data.
        /// </summary>
        private ISkillBlock CreateBlockInstance(SkillBlockData blockData)
        {
            if (!blockRegistry.TryGetValue(blockData.BlockId, out Type blockType))
            {
                LogError($"Unknown block type: {blockData.BlockId}");
                return null;
            }

            try
            {
                var block = (ISkillBlock)Activator.CreateInstance(blockType);
                
                // Apply parameters to block
                ApplyBlockParameters(block, blockData);
                
                return block;
            }
            catch (Exception ex)
            {
                LogError($"Failed to create block instance {blockData.BlockId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Apply parameters to a skill block.
        /// </summary>
        private void ApplyBlockParameters(ISkillBlock block, SkillBlockData blockData)
        {
            // This is a simplified implementation
            // In a full implementation, you would use reflection to set properties
            // based on the parameter names and types
            
            foreach (var parameter in blockData.Parameters)
            {
                // Apply parameter to block (implementation depends on block type)
                // For now, we'll just store them in shared data
                // block.SetParameter(parameter.Name, parameter.Value);
            }
        }

        /// <summary>
        /// Execute a single skill block.
        /// </summary>
        private SkillBlockResult ExecuteBlock(ISkillBlock block, SkillExecutionContext context, SkillBlockData blockData)
        {
            try
            {
                // Check if block can execute
                if (!block.CanExecute(context))
                {
                    LogWarning($"Block {blockData.DisplayName} cannot execute");
                    return SkillBlockResult.Failure;
                }

                // Execute block
                var result = block.Execute(context);
                
                LogInfo($"Executed block {blockData.DisplayName}: {result}");
                return result;
            }
            catch (Exception ex)
            {
                LogError($"Error executing block {blockData.DisplayName}: {ex.Message}");
                return SkillBlockResult.Failure;
            }
        }

        /// <summary>
        /// Interrupt a skill execution.
        /// </summary>
        public void InterruptSkill(SkillInstance skillInstance)
        {
            var execution = activeExecutions.FirstOrDefault(e => e.SkillInstance == skillInstance);
            if (execution != null)
            {
                InterruptExecution(execution);
            }
        }

        /// <summary>
        /// Interrupt an execution.
        /// </summary>
        private void InterruptExecution(SkillExecution execution)
        {
            execution.Interrupt();
            activeExecutions.Remove(execution);
            OnSkillInterrupted?.Invoke(execution.SkillInstance);
        }

        /// <summary>
        /// Stop all skill executions.
        /// </summary>
        public void StopAllSkills()
        {
            foreach (var execution in activeExecutions.ToList())
            {
                InterruptExecution(execution);
            }
        }

        /// <summary>
        /// Get all active skill executions.
        /// </summary>
        public IReadOnlyList<SkillExecution> GetActiveExecutions()
        {
            return activeExecutions.AsReadOnly();
        }

        /// <summary>
        /// Check if a skill is currently being executed.
        /// </summary>
        public bool IsSkillExecuting(SkillInstance skillInstance)
        {
            return activeExecutions.Any(e => e.SkillInstance == skillInstance);
        }

        /// <summary>
        /// Set the event bus for skill events.
        /// </summary>
        public void SetEventBus(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        // Logging methods
        private void LogInfo(string message)
        {
            if (enableDebugLogging)
                Debug.Log($"[SkillExecutor] {message}");
        }

        private void LogWarning(string message)
        {
            if (enableDebugLogging)
                Debug.LogWarning($"[SkillExecutor] {message}");
        }

        private void LogError(string message)
        {
            if (enableDebugLogging)
                Debug.LogError($"[SkillExecutor] {message}");
        }
    }

    /// <summary>
    /// Represents an active skill execution.
    /// </summary>
    public class SkillExecution
    {
        public SkillInstance SkillInstance { get; private set; }
        public SkillExecutionContext Context { get; private set; }
        public float StartTime { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsInterrupted { get; private set; }
        public bool IsFailed { get; private set; }

        public SkillExecution(SkillInstance skillInstance, SkillExecutionContext context)
        {
            SkillInstance = skillInstance;
            Context = context;
            StartTime = Time.time;
        }

        public void Update(float deltaTime)
        {
            // Update context
            Context.ExecutionTime = Time.time;
            Context.DeltaTime = deltaTime;
        }

        public void Complete()
        {
            IsCompleted = true;
        }

        public void Fail()
        {
            IsFailed = true;
            IsCompleted = true;
        }

        public void Interrupt()
        {
            IsInterrupted = true;
            IsCompleted = true;
        }
    }
} 