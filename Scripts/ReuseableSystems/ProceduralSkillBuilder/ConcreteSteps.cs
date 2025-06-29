// --- Step Implementations (DoStep, ThenStep, IfStep, ForEachTargetStep) ---
using System;
using UnityEngine;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class DoStep: ISkillStep
    {
        private readonly Action<ExecutionContext> action;

        public DoStep(Action<ExecutionContext> action) => this.action = action;

        public void Execute(ExecutionContext ctx) => action(ctx);

        public string Description => "Do action based on context.)";
    }

    public class ThenStep : ISkillStep
    {
        private readonly Action<ExecutionContext> action;

        public ThenStep(Action<ExecutionContext> action) => this.action = action;

        public void Execute(ExecutionContext ctx) => action(ctx);

        public string Description => "Perform post-damage logic (status, callbacks, etc.)";
    }

    public class IfStep : ISkillStep
    {
        private readonly Func<ExecutionContext, bool> condition;
        private readonly Action<ExecutionContext> action;

        public IfStep(Func<ExecutionContext, bool> condition, Action<ExecutionContext> action)
        {
            this.condition = condition;
            this.action = action;
        }

        public void Execute(ExecutionContext ctx)
        {
            if (condition(ctx)) action(ctx);
        }

        public string Description => $"Conditional logic block: executes only if condition is met";
    }

    public class ForEachTargetStep : ISkillStep
    {
        private readonly Func<GameObject, Skill> perTargetBuilder;

        public ForEachTargetStep(Func<GameObject, Skill> builder)
        {
            perTargetBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void Execute(ExecutionContext ctx)
        {
            foreach (var target in ctx.Targets)
            {
                if (target == null)
                    continue;

                var skill = perTargetBuilder(target);
                if (skill == null)
                    continue;

                // Run the sub‐skill *from* this individual target,
                // and use a Single‐target finder so it only hits itself (or whatever you choose)
                skill
                    .From(target)
                    .Target(b => b.Single(target))
                    .Execute();
            }
        }

        public string Description => "Iterate over each target and apply sub-skill";
    }

    public class PlayVFXStep : ISkillStep
    {
        public PlayVFXStep()
        {
            // Initialize any visual effect settings here if needed
        }

        public void Execute(ExecutionContext ctx)
        {
        }
        
        public string Description => "Play visual effects";
    }

    public class PlaySFXStep : ISkillStep
    {
        public PlaySFXStep()
        {
            // Initialize any sound effect settings here if needed
        }

        public void Execute(ExecutionContext ctx)
        {
        }
        
        public string Description => "Play sound effects";
    }
}
