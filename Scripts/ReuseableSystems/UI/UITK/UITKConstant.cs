namespace VT.ReusableSystems.UI.UITK
{
    public enum UITKConstantType
    {
        ElementName,
        Selector
    }

    public struct UITKConstant
    {
        public UITKConstant(UITKConstantType type, string value)
        {
            Type = type;
            Value = value;
        }

        public readonly UITKConstantType Type;
        public readonly string Value;
    }
}