using UnityEngine;

public class ResizableTextAreaAttribute : PropertyAttribute
{
    public readonly float minHeight;
    public readonly float maxHeight;

    public ResizableTextAreaAttribute(float minHeight = 60f, float maxHeight = 300f)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
    }
}
