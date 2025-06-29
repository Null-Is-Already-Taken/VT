using System;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    /// <summary>
    /// Skill step that executes an IAction.
    /// </summary>
    public class ActionStep : ISkillStep
    {
        private readonly IAction action;

        public ActionStep(IAction action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Execute(ExecutionContext context)
        {
            Debug.Log($"[ActionStep] Executing action: {action.GetType().Name} - {action.Description}");
            Debug.Log($"[ActionStep] Context source: {context.Source?.name ?? "null"}");
            Debug.Log($"[ActionStep] Context targets count: {context.Targets?.Count ?? 0}");
            
            try
            {
                action.Execute(context);
                Debug.Log($"[ActionStep] Successfully executed action: {action.GetType().Name}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing action {action.GetType().Name}: {ex.Message}");
            }
        }

        public string Description => action.Description;
    }
} 