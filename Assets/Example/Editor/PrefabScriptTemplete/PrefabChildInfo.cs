using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Prefab的字段信息记录
/// </summary>
[Serializable]
public class PrefabChildInfo
{
    [HideInInspector]
    public string childPath;

    [DisplayAsString]
    [ShowInInspector]
    [HideLabel]
    [CustomIndent("GetIndent")]
    [HorizontalGroup]
    [PropertyOrder(0)]
    public string ChildName
    {
        get
        {
            if (string.IsNullOrEmpty(childPath))
            {
                return string.Empty;
            }
            else
            {
                return childPath.Split('/').Last();
            }
        }
        set { }
    }

#if UNITY_EDITOR
    [HideLabel]
    [HideReferenceObjectPicker]
    [HorizontalList]
    [HorizontalGroup(MaxWidth = 200, MinWidth = 100)]
#endif
    [PropertyOrder(1)]
    public List<ComponentInfo> componentInfos;

    [NonSerialized]
    public string SelfName;

    [NonSerialized]
    public GameObject selfObj;

    int GetIndent()
    {
        return childPath.Count(c=>c=='/');
    }

    public void ClearDisableComponents()
    {
        componentInfos?.RemoveAll(componentInfo => !componentInfo.enable);
    }

    public bool Enable()
    {
        return componentInfos?.Find(info => info.enable) != null;
    }

    /*public void AddComponentInfo(params Component[] components)
    {
        for (var i = 0; i < components.Length; i++)
        {
            var component = components[i];
            AddComponentInfo(component, ()=>{});
        }
    }

    public void AddComponentInfo(Component component, Action onValueChange)
    {
        AddComponentInfo(component.GetType(), onValueChange);
    }*/

    public void AddComponentInfo(Type componentType, bool isEnable = false)
    {
        componentInfos?.Add(new ComponentInfo(componentType, isEnable));
    }

    public bool TryGetComponentInfoWithName(string name, out ComponentInfo info)
    {
        info = null;
        var cs =this.componentInfos.Where(item => item.TypeName == name).ToArray();
        if (cs.Length == 0) return false;
        else info = cs[0];
        return true;
    }

    public bool Contains(string componentName)
    {
        return componentInfos?.Find(info => info.TypeName == componentName) != null;
    }

    public bool Contains(Component component)
    {
        return Contains(component.GetType().Name);
    }

    public bool IsEnableComponent(Component component)
    {
        var componentName = component.GetType().Name;
        return componentInfos?.Find(info => info.enable && info.TypeName == componentName) != null;
    }
}