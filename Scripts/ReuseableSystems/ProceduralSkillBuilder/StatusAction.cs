using UnityEngine;
using System;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    /// <summary>
    /// Action that applies status effects to targets.
    /// </summary>
    public class StatusAction : MultiTargetAction
    {
        private string statusName;
        private float duration;
        private bool stackable = false;
        private int maxStacks = 1;

        public StatusAction()
        {
        }

        public StatusAction(string statusName, float duration, bool stackable = false, int maxStacks = 1)
        {
            this.statusName = statusName;
            this.duration = duration;
            this.stackable = stackable;
            this.maxStacks = maxStacks;
        }

        public override void Execute(ExecutionContext context)
        {
            var logger = context.Get<ISkillLogger>(ContextKeys.Logger, null);
            var targets = GetTargets(context);

            foreach (var target in targets)
            {
                if (target == null) continue;

                target.ApplyStatus(statusName, duration);
                logger?.LogTag("Status", $"{target.name} receives {statusName} for {duration}s");
            }
        }

        public override string Description => $"Apply {statusName} for {duration}s" +
            (stackable ? $" (stackable, max {maxStacks})" : "");

        // Fluent API methods for building the action
        public StatusAction Status(string name)
        {
            statusName = name;
            return this;
        }

        public StatusAction Duration(float seconds)
        {
            duration = seconds;
            return this;
        }

        public StatusAction Stackable(bool stackable = true, int maxStacks = 1)
        {
            this.stackable = stackable;
            this.maxStacks = maxStacks;
            return this;
        }
    }
} 