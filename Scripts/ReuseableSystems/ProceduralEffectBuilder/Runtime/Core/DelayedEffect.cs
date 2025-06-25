using System;
using System.Threading.Tasks;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core
{
    /// <summary>
    /// Waits for a given duration before executing the wrapped effect.
    /// </summary>
    public class DelayedEffect : IEffect
    {
        private readonly IEffect inner;
        private readonly float delaySeconds;

        public DelayedEffect(IEffect inner, float delaySeconds)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.delaySeconds = delaySeconds;
        }

        public async Task ExecuteAsync()
        {
            if (delaySeconds > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }

            await inner.ExecuteAsync();
        }

        public IEffect And(IEffect next)
        {
            return new ChainedEffect(this, next);
        }
    }
}
