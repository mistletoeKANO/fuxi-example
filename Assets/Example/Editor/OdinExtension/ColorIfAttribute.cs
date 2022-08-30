using System;

public class ColorIfAttribute : Attribute
{
    public string Color;

    public string Condition;

    public ColorIfAttribute(string color, string condition)
    {
        this.Color = color;
        this.Condition = condition;
    }
}