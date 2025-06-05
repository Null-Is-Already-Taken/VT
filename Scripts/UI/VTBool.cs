using UnityEngine;
using UnityEngine.Events;

namespace VT.UI
{
    [System.Serializable]
    public class VTBool
    {
        [SerializeField] private bool value;
        public UnityEvent<bool> OnValueChanged;

        public VTBool(bool initialValue = false)
        {
            value = initialValue;
        }

        public bool Value
        {
            get => value;
            set
            {
                if (this.value == value) return;
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }

        public void Toggle() => Value = !value;

        public override string ToString() => value.ToString();

        public static implicit operator bool(VTBool vtBool) => vtBool.value;
    }
}
