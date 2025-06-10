using VT.Logger;

namespace VT.UI
{
    public class VTBoolHook : IBoolHook
    {
        private VTBool boolField;
        private ILogger logger;
        private readonly string className = nameof(VTBoolHook);

        // Start is called before the first frame update
        public VTBoolHook(VTBool boolField, ILogger logger = null)
        {
            this.boolField = boolField;
            this.logger = logger;
        }

        public void UpdateBool(bool value)
        {
            if (!boolField)
            {
                logger?.LogWarning($"[UpdateBool] There is no reference for bool field (VTBool).", className);
                return;
            }

            boolField.Value = value;
        }
    }
}
