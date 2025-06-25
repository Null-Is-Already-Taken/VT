using System;
using System.Threading.Tasks;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core
{
    /// <summary>
    /// Wraps another effect and only executes it if the condition is true.
    /// </summary>
    public class ConditionalEffect : IEffect
    {
        private readonly IEffect inner;
        private readonly Func<bool> condition;

        public ConditionalEffect(IEffect inner, Func<bool> condition)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public async Task ExecuteAsync()
        {
            if (condition())
            {
                await inner.ExecuteAsync();
            }
        }

        public IEffect And(IEffect next)
        {
            return new ChainedEffect(this, next);
        }
    }
}
