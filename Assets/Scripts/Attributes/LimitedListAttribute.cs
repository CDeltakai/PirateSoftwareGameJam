using UnityEngine;

public class LimitedListAttribute : PropertyAttribute
{
    public int MaxElements { get; private set; }

    public LimitedListAttribute(int maxElements)
    {
        MaxElements = maxElements;
    }
}