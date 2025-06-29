using System;
using System.Collections.Generic;
using UnityEngine;
using VT.Extensions;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class Skill
    {
        private readonly string name;
        private readonly ExecutionContext context;
        private readonly List<ISkillStep> steps = new();
        private ITargetFinder targetFinder;

        private Skill(string name)
        {
            this.name = name;
            context = new ExecutionContext();
        }

        public static Skill Build(string name) => new(name);

        public Skill From(GameObject source)
        {
            context.Source = source;
            Debug.Log($"[Skill] Set source for skill '{name}': {source?.name ?? "null"}");
            return this;
        }

        public Skill Do(Action<ExecutionContext> action)
        {
            steps.Add(new DoStep(action));
            Debug.Log($"[Skill] Added Do step to skill '{name}'. Total steps: {steps.Count}");
            return this;
        }

        /// <summary>
        /// Execute an action using the new IAction system.
        /// </summary>
        public Skill Do(IAction action)
        {
            steps.Add(new ActionStep(action));
            Debug.Log($"[Skill] Added IAction step to skill '{name}': {action.Description}. Total steps: {steps.Count}");
            return this;
        }

        public Skill Then(Action<ExecutionContext> action)
        {
            steps.Add(new ThenStep(action));
            Debug.Log($"[Skill] Added Then step to skill '{name}'. Total steps: {steps.Count}");
            return this;
        }

        /// <summary>
        /// Execute an action after the previous step using the new IAction system.
        /// </summary>
        public Skill Then(IAction action)
        {
            steps.Add(new ActionStep(action));
            Debug.Log($"[Skill] Added IAction Then step to skill '{name}': {action.Description}. Total steps: {steps.Count}");
            return this;
        }

        public Skill If(Func<ExecutionContext, bool> condition, Action<ExecutionContext> ifTrue)
        {
            steps.Add(new IfStep(condition, ifTrue));
            return this;
        }

        public Skill ForEachTarget(Func<GameObject, Skill> builderPerTarget)
        {
            steps.Add(new ForEachTargetStep(builderPerTarget));
            return this;
        }

        public Skill PlayDamageNumber(float amount)
        {
            steps.Add(new PlayVFXStep());
            return this;
        }

        public Skill Target(Func<TargetFinder, TargetFinder> builderFunc)
        {
            var builder = builderFunc(new TargetFinder());
            targetFinder = builder.Build();
            Debug.Log($"[Skill] Set target finder for skill '{name}': {targetFinder.Description}");
            return this;
        }

        public void Execute()
        {
            Debug.Log($"[Skill] Executing skill '{name}' with {steps.Count} steps");
            
            if (context.Source == null)
            {
                Debug.LogError($"Skill '{name}' has no source set. Cannot execute.");
                return;
            }

            if (context.Targets.IsNullOrEmpty() && targetFinder != null)
            {
                context.Targets = targetFinder.ResolveTargets(context.Source);
                Debug.Log($"[Skill] Resolved {context.Targets?.Count ?? 0} targets for skill '{name}'");
            }

            Debug.Log($"[Skill] Executing {steps.Count} steps for skill '{name}'");
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                Debug.Log($"[Skill] Executing step {i + 1}/{steps.Count}: {step.Description}");
                step.Execute(context);
            }
            
            Debug.Log($"[Skill] Finished executing skill '{name}'");
        }

        public Skill Log(string name)
        {
            context.Set(ContextKeys.Logger, new SkillLogger(name));
            return this;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"Skill: {name}");
            sb.AppendLine($"- Source: {context?.Source?.name ?? "null"}");
            // show the finder description

            if (targetFinder != null)
                sb.AppendLine($"- TargetFinder: {targetFinder.Description}");

            var targetNames = context.Targets?.ConvertAll(t => t?.name ?? "null") ?? new();
            sb.AppendLine($"- Targets ({targetNames.Count}): [{string.Join(", ", targetNames)}]");

            sb.AppendLine("- Variables:");
            if (context?.Variables?.Count > 0)
            {
                foreach (var kv in context.Variables)
                    sb.AppendLine($"    {kv.Key}: {kv.Value}");
            }
            else
            {
                sb.AppendLine("    (none)");
            }

            sb.AppendLine($"- Steps ({steps.Count}):");
            if (steps.Count > 0)
            {
                for (int i = 0; i < steps.Count; i++)
                    sb.AppendLine($"    {i + 1}. {steps[i].Description}");
            }
            else
            {
                sb.AppendLine("    (none)");
            }

            return sb.ToString();
        }
    }
}