using TMPro;
using VT.Utils.Logger;

namespace VT.UI
{
    public class TMPTextHook : ITextHook
    {
        private TMP_Text textField;
        private ILogger logger;
        private readonly string className = nameof(TMPTextHook);

        // Start is called before the first frame update
        public TMPTextHook(TMP_Text textField, ILogger logger = null)
        {
            this.textField = textField;
            this.logger = logger;
        }

        public void UpdateText(string text)
        {
            if (!textField)
            {
                logger?.LogWarning($"[UpdateText] There is no reference for text field (TMP).", className);
                return;
            }

            textField.text = text;
        }
    }
}
