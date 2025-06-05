using UnityEngine.UI;
using VT.Utils.Logger;

namespace VT.UI
{
    public class ToggleHook : IBoolHook
    {
        private Toggle toggleField;
        private ILogger logger;
        private readonly string className = nameof(ToggleHook);

        // Start is called before the first frame update
        public ToggleHook(Toggle toggleField, ILogger logger = null)
        {
            this.toggleField = toggleField;
            this.logger = logger;
        }

        public void UpdateBool(bool value)
        {
            if (!toggleField)
            {
                logger?.LogWarning($"[UpdateText] There is no reference for text field (TMP).", className);
                return;
            }

            toggleField.isOn = value;
        }
    }
}
