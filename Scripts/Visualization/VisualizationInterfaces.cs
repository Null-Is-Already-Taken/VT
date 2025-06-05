#if UNITY_EDITOR
namespace VT.Visualization
{
    public interface IVisualizer
    {
        void DrawGizmos();
    }

    public interface IDraggable
    {
        /// <summary>
        /// Draws interactive handles to modify internal state in Scene view.
        /// </summary>
        void DrawHandles();
    }
}
#endif
