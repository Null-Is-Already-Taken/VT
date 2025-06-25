using System.Threading.Tasks;

namespace VT.ReusableSystems.ProceduralEffectBuilder.Runtime.Interfaces
{
    /// <summary>
    /// Represents a compiled, executable effect that may occur over time.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Asynchronously executes the effect logic.
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Chains this effect with another, executing them in order.
        /// </summary>
        IEffect And(IEffect next);
    }
}
