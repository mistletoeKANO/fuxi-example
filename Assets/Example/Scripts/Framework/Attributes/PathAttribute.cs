using System;

public class PathAttribute : Attribute
{
    public string PrefabPath;
    
    public PathAttribute(string prefabPath)
    {
        PrefabPath = prefabPath;
    }
}