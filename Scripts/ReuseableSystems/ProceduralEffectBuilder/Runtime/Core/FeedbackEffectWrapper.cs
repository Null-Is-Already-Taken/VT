using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Definitions;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core
{
    /// <summary>
    /// Wraps an IEffect and triggers one or more feedback decorators (e.g., VFX, SFX) when executed.
    /// </summary>
    public class FeedbackEffectWrapper : IEffect
    {
        private readonly IEffect inner;
        private readonly List<FeedbackDecorator> feedbacks;
        private readonly GameObject source;
        private readonly GameObject target;

        public FeedbackEffectWrapper(
            IEffect inner,
            List<FeedbackDecorator> feedbacks,
            GameObject source,
            GameObject target)
        {
            this.inner = inner;
            this.feedbacks = feedbacks;
            this.source = source;
            this.target = target;
        }

        public async Task ExecuteAsync()
        {
            if (feedbacks != null)
            {
                foreach (var feedback in feedbacks)
                {
                    feedback?.Play(source, target);
                }
            }

            await inner.ExecuteAsync();
        }

        public IEffect And(IEffect next)
        {
            return new ChainedEffect(this, next);
        }
    }
}
