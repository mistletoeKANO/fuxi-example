using System;
using Sirenix.OdinInspector;

[Serializable]
[InlineEditor]
public class AutoFillFieldRule
{
    public const char flag = '*';
    
    [ValidateInput("IsKeyValid", "error")]
    [HorizontalGroup]
    [HideLabel]
    [DelayedProperty]
    public string key;

    [HorizontalGroup]
    [HideLabel]
    public string type;

    bool IsKeyValid()
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        if (key.Length < 2)
        {
            return false;
        }

        if (key[0] != flag && key[key.Length - 1] != flag)
        {
            return false;
        }

        if (key.Contains(" "))
        {
            return false;
        }
        return true;
    }
}