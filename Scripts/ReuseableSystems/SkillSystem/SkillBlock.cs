using System;
using System.Collections.Generic;
using UnityEngine;

namespace VT.SkillSystem
{
    public interface ISkillBuildingBlock
    {
        bool CanFollow(ISkillBuildingBlock previousBlock);
        bool CanBeFollowedBy(ISkillBuildingBlock nextBlock);
        void Execute(SkillContext context);
    }

    /// <summary>
    /// Base class for all skill building blocks
    /// </summary>
    public abstract class SkillBlock : ISkillBuildingBlock
    {
        [Header("Grammar Rules")]
        [SerializeField] protected List<Type> validNextBlocks = new();
        [SerializeField] protected List<Type> validPreviousBlocks = new();
        [SerializeField] protected bool isModifier = false;
        [SerializeField] protected bool canStartSkill = false;
        
        public bool IsModifier => isModifier;
        public bool CanStartSkill => canStartSkill;
        
        /// <summary>
        /// Check if this block can follow the given block
        /// </summary>
        public virtual bool CanFollow(ISkillBuildingBlock previousBlock)
        {
            if (previousBlock == null) return canStartSkill;
            return validPreviousBlocks.Contains(previousBlock.GetType());
        }
        
        /// <summary>
        /// Check if the given block can follow this block
        /// </summary>
        public virtual bool CanBeFollowedBy(ISkillBuildingBlock nextBlock)
        {
            if (nextBlock == null) return true;
            return validNextBlocks.Contains(nextBlock.GetType());
        }
        
        /// <summary>
        /// Execute the block's logic
        /// </summary>
        public abstract void Execute(SkillContext context);
    }
} 