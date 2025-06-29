using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    /// <summary>
    /// Base interface for all skill actions that can be executed within a skill context.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Executes the action using the provided execution context.
        /// </summary>
        /// <param name="context">The execution context containing source, targets, and variables.</param>
        void Execute(ExecutionContext context);
        
        /// <summary>
        /// Human-readable description of what this action does.
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Base class for actions that operate on a single target.
    /// </summary>
    public abstract class SingleTargetAction : IAction
    {
        public abstract void Execute(ExecutionContext context);
        public abstract string Description { get; }
        
        /// <summary>
        /// Gets the primary target for this action. Override if you need custom target selection logic.
        /// </summary>
        protected virtual GameObject GetTarget(ExecutionContext context)
        {
            return context.Targets?.Count > 0 ? context.Targets[0] : null;
        }
    }

    /// <summary>
    /// Base class for actions that operate on multiple targets.
    /// </summary>
    public abstract class MultiTargetAction : IAction
    {
        public abstract void Execute(ExecutionContext context);
        public abstract string Description { get; }
        
        /// <summary>
        /// Gets all targets for this action. Override if you need custom target selection logic.
        /// </summary>
        protected virtual System.Collections.Generic.List<GameObject> GetTargets(ExecutionContext context)
        {
            return context.Targets ?? new System.Collections.Generic.List<GameObject>();
        }
    }
} 