using System;
using UnityEngine;

namespace VT.ReusableSystems.SkillSystem.Core
{
    /// <summary>
    /// Represents a single, atomic unit of skill functionality.
    /// Skill blocks are the fundamental building blocks that can be composed into complex skills.
    /// </summary>
    public interface ISkillBlock
    {
        /// <summary>
        /// Unique identifier for this skill block type.
        /// </summary>
        string BlockId { get; }
        
        /// <summary>
        /// Human-readable name for this skill block.
        /// </summary>
        string DisplayName { get; }
        
        /// <summary>
        /// Description of what this skill block does.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Whether this block can be executed.
        /// </summary>
        bool CanExecute(SkillExecutionContext context);
        
        /// <summary>
        /// Execute this skill block.
        /// </summary>
        /// <param name="context">The execution context containing all necessary data</param>
        /// <returns>Result of the execution</returns>
        SkillBlockResult Execute(SkillExecutionContext context);
        
        /// <summary>
        /// Validate this skill block's configuration.
        /// </summary>
        /// <returns>Validation result</returns>
        ValidationResult Validate();
    }

    /// <summary>
    /// Result of a skill block execution.
    /// </summary>
    public enum SkillBlockResult
    {
        /// <summary>
        /// Block executed successfully.
        /// </summary>
        Success,
        
        /// <summary>
        /// Block execution failed.
        /// </summary>
        Failure,
        
        /// <summary>
        /// Block execution was interrupted.
        /// </summary>
        Interrupted,
        
        /// <summary>
        /// Block execution is still in progress (for async blocks).
        /// </summary>
        InProgress
    }

    /// <summary>
    /// Result of skill block validation.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }

        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        public static ValidationResult Failure(string error) => new ValidationResult { IsValid = false, ErrorMessage = error };
        public static ValidationResult Warning(string warning) => new ValidationResult { IsValid = true, WarningMessage = warning };
    }
} 