using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Field)]
[Conditional("UNITY_EDITOR")]
public class ToggleColorButtonAttribute : Attribute
{
    public string enableColor;

    public string disableColor;

    public string enableString;

    public string disableString;

    public ToggleColorButtonAttribute()
    {
    }

    public ToggleColorButtonAttribute(string label)
    {
        enableString = label;
        disableString = label;
    }
    
    public ToggleColorButtonAttribute(string enableString, string disableString)
    {
        this.enableString = enableString;
        this.disableString = disableString;
    }

    public ToggleColorButtonAttribute(string label, string enableColor, string disableColor)
    {
        enableString = label;
        disableString = label;
        this.enableColor = enableColor;
        this.disableColor = disableColor;
    }
    
    public ToggleColorButtonAttribute(string enableString, string disableString, string enableColor, string disableColor)
    {
        this.enableString = enableString;
        this.disableString = disableString;
        this.enableColor = enableColor;
        this.disableColor = disableColor;
    }
}