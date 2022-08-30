using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[TypeInfoBox("hello world")]
public abstract class PrefabViewScriptGenerateRule : ScriptableObject
{
    public virtual void Generate(PrefabInfo prefabInfo){}

    public virtual void GenerateItemView(PrefabInfo prefabInfo){}
}