using System.Threading.Tasks;
using VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Core
{
    public class NoOpEffect : IEffect
    {
        public Task ExecuteAsync() => Task.CompletedTask;
        public IEffect And(IEffect next) => next;
    }
}