using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ComponentInfo
{
    private static readonly Dictionary<Type, string> cs2SimpleName = new Dictionary<Type, string>()
    {
        [typeof(RectTransform)] = "Rt",
        [typeof(Canvas)] = "Cav",
        [typeof(CanvasGroup)] = "CavGp",
        [typeof(Button)] = "Btn",
        [typeof(Image)] = "Img",
        [typeof(Input)] = "Input",
        [typeof(Toggle)] = "Tog",
        [typeof(ToggleGroup)] = "TogGroup",
        [typeof(Slider)] = "Slider",
        [typeof(TMP_Dropdown)] = "Drop",
        [typeof(UIGroup)] = "Group",
        [typeof(GridLayoutGroup)] = "GLayout",
        [typeof(TMP_InputField)] = "InpField",
        [typeof(HorizontalLayoutGroup)] = "HLayout",
        [typeof(VerticalLayoutGroup)] = "VLayout",
        [typeof(TextMeshProUGUI)] = "Text",
    };

    public static void CheckAndReplaceSimName(ref string name)
    {
        foreach (var value in cs2SimpleName.Values)
        {
            if (!name.StartsWith(value)) continue;
            name = name.Replace(value + "_", "");
            return;
        }
    }

    public static readonly List<Type> defaultEnableType = new List<Type>()
    {
        typeof(Button),
    };

    /// <summary>
    /// 忽略组件列表
    /// 可以添加到预制体但，但不会在组件面板显示
    /// </summary>
    public static readonly List<Type> ignoreComponents = new List<Type>
    {
        typeof(CanvasRenderer),
    };

    [SerializeField]
    [HideInInspector]
    string typeName;

    public string TypeName
    {
        get { return typeName; }
    }

    public Type type;

    public string SimpleName
    {
        get
        {
            if (null == type)
            {
                return typeName;
            }
            if (cs2SimpleName.ContainsKey(this.type))
            {
                return cs2SimpleName[this.type];
            }
            return typeName;
        }
    }

    [ToggleColorButton("SimpleName", "EnableColor", "DisableColor")]
    [OnValueChanged("OnEnableChanged")]
    public bool enable;

    public ComponentInfo(Type type, bool enable = false)
    {
        this.type = type;
        this.typeName = type.Name;
        this.enable = enable;
    }

    public bool IsDefaultComponent()
    {
        return typeName == "Transform" || typeName == "RectTransform";
    }

    public override string ToString()
    {
        return typeName;
    }

    /// <summary>
    /// 是否被禁止
    /// </summary>
    /// <returns></returns>
    public bool IsProhibit()
    {
        return null == type;
    }

#if UNITY_EDITOR
    Color EnableColor()
    {
        return IsProhibit() ? Color.red : Color.white;
    }

    Color DisableColor()
    {
        return IsProhibit() ? Color.red : Color.gray;
    }

    void OnEnableChanged()
    {
        if (IsProhibit())
        {
            enable = false;
        }
    }
#endif
}