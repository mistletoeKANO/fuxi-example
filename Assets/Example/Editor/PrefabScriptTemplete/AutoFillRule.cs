using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class AutoFillRule : ScriptableObject
{
    [LabelText(" ")]
    [LabelWidth(20)]
    [InlineProperty]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowIndexLabels = true)]
    [ValidateInput("IsValid")]
    public AutoFillFieldRule[] autoFillFieldRule;
    
    bool IsValid(ref string errorMessage)
    {
        HashSet<string> set = new HashSet<string>();
        for (var i = 0; i < autoFillFieldRule.Length; i++)
        {
            var fillFieldRule = autoFillFieldRule[i];
            if (set.Contains(fillFieldRule.key))
            {
                errorMessage = $"same key in {i}:{fillFieldRule.key}";
                return false;
            }

            if (set.Contains(fillFieldRule.type))
            {
                errorMessage = $"same type in {i}:{fillFieldRule.type}";
                return false;
            }

            set.Add(fillFieldRule.key);
            set.Add(fillFieldRule.type);
        }

        return true;
    }
}