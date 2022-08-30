using System;

public class CustomIndentAttribute : Attribute
{
    public string getIndent;

    public CustomIndentAttribute(string getIndent)
    {
        this.getIndent = getIndent;
    }
}