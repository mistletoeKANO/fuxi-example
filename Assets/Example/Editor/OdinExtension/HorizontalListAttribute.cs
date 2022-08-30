using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.Field)]
[Conditional("UNITY_EDITOR")]
[DontApplyToListElements]
public class HorizontalListAttribute : Attribute
{
    
}