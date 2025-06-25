using System;
using System.Threading.Tasks;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core
{
    /// <summary>
    /// Executes two effects sequentially, respecting async flow.
    /// </summary>
    public class ChainedEffect : IEffect
    {
        private readonly IEffect current;
        private readonly IEffect next;

        public ChainedEffect(IEffect current, IEffect next)
        {
            this.current = current ?? throw new ArgumentNullException(nameof(current));
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task ExecuteAsync()
        {
            await current.ExecuteAsync();
            await next.ExecuteAsync();
        }

        public IEffect And(IEffect nextEffect)
        {
            return new ChainedEffect(this, nextEffect);
        }
    }
}
