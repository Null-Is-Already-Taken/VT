using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VT.SkillSystem
{
    /// <summary>
    /// A skill composed of building blocks that follow grammatical rules
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "VT/Skill System/Skill")]
    public class Skill : ScriptableObject
    {
        [Header("Skill Configuration")]
        [SerializeField] private string skillName = "New Skill";
        [SerializeField] private List<SkillBlock> blocks = new();
        [SerializeField] private float baseMagnitude = 10f;
        [SerializeField] private float baseDuration = 1f;
        [SerializeField] private float baseRange = 5f;
        
        public string SkillName => skillName;
        public IReadOnlyList<SkillBlock> Blocks => blocks.AsReadOnly();
        
        /// <summary>
        /// Validate that the skill follows grammatical rules
        /// </summary>
        public bool IsValid()
        {
            if (blocks.Count == 0) return false;
            
            // First block must be able to start a skill
            if (!blocks[0].CanStartSkill) return false;
            
            // Check each block can follow the previous one
            for (int i = 1; i < blocks.Count; i++)
            {
                if (!blocks[i].CanFollow(blocks[i - 1]))
                {
                    Debug.LogError($"Block {i} ({blocks[i].GetType().Name}) cannot follow block {i-1} ({blocks[i-1].GetType().Name})");
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Execute the skill with the given context
        /// </summary>
        public void Execute(GameObject caster, GameObject target = null)
        {
            if (!IsValid())
            {
                Debug.LogError($"Skill '{skillName}' is not valid!");
                return;
            }
            
            var context = new SkillContext
            {
                Caster = caster,
                Target = target,
                Magnitude = baseMagnitude,
                Duration = baseDuration,
                Range = baseRange
            };
            
            // Execute blocks in order
            foreach (var block in blocks)
            {
                block.Execute(context);
            }
        }
        
        /// <summary>
        /// Add a block to the skill if it's grammatically valid
        /// </summary>
        public bool AddBlock(SkillBlock block)
        {
            if (block == null) return false;
            
            // Check if block can be added
            if (blocks.Count == 0)
            {
                if (!block.CanStartSkill) return false;
            }
            else
            {
                if (!block.CanFollow(blocks[blocks.Count - 1])) return false;
            }
            
            blocks.Add(block);
            return true;
        }
        
        /// <summary>
        /// Remove a block at the specified index
        /// </summary>
        public void RemoveBlock(int index)
        {
            if (index >= 0 && index < blocks.Count)
            {
                blocks.RemoveAt(index);
            }
        }
        
        /// <summary>
        /// Get a summary of the skill's grammar
        /// </summary>
        public string GetGrammarSummary()
        {
            if (blocks.Count == 0) return "Empty skill";
            
            var summary = $"Skill: {skillName}\n";
            summary += $"Blocks: {blocks.Count}\n";
            summary += $"Valid: {IsValid()}\n\n";
            
            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                summary += $"{i}: {block.GetType().Name}";
                if (block.IsModifier) summary += " (Modifier)";
                if (block.CanStartSkill) summary += " (Can Start)";
                summary += "\n";
            }
            
            return summary;
        }
    }
} 