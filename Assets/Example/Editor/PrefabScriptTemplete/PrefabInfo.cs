using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 职责：记录预制体的哪些子物体上的哪些组件需要被程序使用。
/// </summary>
[CreateAssetMenu(fileName = "NewTemplate", menuName = "ScriptGenerateRule/Template")]
public class PrefabInfo : ScriptableObject
{
    /// <summary>
    /// 预制体路径
    /// </summary>
    public string prefabPath;

    public string prefabGUID;
    
    public List<PrefabChildInfo> childrenInfos;

    bool isReadonly;
    public string PrefabPath => $"Assets/Example/BundleResource/{prefabPath}";

    [NonSerialized][HideInInspector]
    public UIFormOperation uiFormOperation;

    public string PrefabName
    {
        get { return Path.GetFileNameWithoutExtension(PrefabPath); }
    }

#if UNITY_EDITOR

    void PingPrefab()
    {
        Ping(PrefabPath);
    }

    /// <summary>
    /// 检查预制体路径是否存在
    /// 检查配置的所有子物体路径是否存在。
    /// </summary>
    /// <param name="prefabFields"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    bool IsValid(List<PrefabChildInfo> prefabFields, ref string errorMessage)
    {
        if (File.Exists(PrefabPath))
        {
            var t = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath).transform;
            for (var i = 0; i < prefabFields.Count; i++)
            {
                var info = prefabFields[i];
                var child = t.Find(info.childPath);
                if (null == child)
                {
                    errorMessage = $"{i}:{info.childPath}";
                    return false;
                }
            }

            /*if (!string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = errorMessage.Insert(0, $"以下子物体命名冲突:\n");
                return false;
            }*/
        }

        return true;
    }

    /// <summary>
    /// Ping 指定路径
    /// </summary>
    /// <param name="aimPath"></param>
    void Ping(string aimPath)
    {
        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(aimPath);
        if (obj)
        {
            EditorGUIUtility.PingObject(obj);
        }
        else
        {
            EditorUtility.DisplayDialog("Message", $"项目中没有：{aimPath}", "ok");
        }
    }

    void ToggleReadonly()
    {
        if (isReadonly)
        {
            ViewComponents();
        }
        else
        {
            HideComponents();
        }
    }

    /// <summary>
    /// 获取最新预制体上的所有组件
    /// 根据当前的记录来设置获取到的组件的可用性
    /// 用最新的记录覆盖当前记录。
    /// </summary>
    void ViewComponents()
    {
        isReadonly = false;
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (null == obj)
        {
            Debug.LogError($"{PrefabPath} is null");
            return;
        }
        var t = obj.transform;
        this.uiFormOperation = obj.GetComponent<UIFormOperation>();
        if (null == t)
        {
            return;
        }
        var allFields = GetList(t);
        for (var i = 0; i < allFields.Count; i++)
        {
            var field = allFields[i];
            if (null == field)
            {
                continue;
            }
            var child = t.Find(field.childPath);
            if (null == child)
            {
                continue;
            }

            var currentField = childrenInfos.Find(prefabField => prefabField.childPath == field.childPath);
            var components = child.GetComponents<Component>();
            foreach (var component in components)
            {
                var componentName = component.GetType();
                if (ComponentInfo.ignoreComponents.Contains(componentName))
                {
                    continue;
                }
                bool isEnable = null != currentField && currentField.IsEnableComponent(component);
                //isEnable = ComponentInfo.defaultEnableType.Contains(componentName);
                field.AddComponentInfo(componentName, isEnable);
            }
        }

        childrenInfos = allFields;
    }

    void HideComponents()
    {
        isReadonly = true;
        childrenInfos?.RemoveAll(field => !field.Enable());
        childrenInfos?.ForEach(field => { field.ClearDisableComponents(); });
    }

    public static bool IsDefaultComponent(Component component)
    {
        return component is Transform;
    }

    List<PrefabChildInfo> GetList(Transform t, string transformName = "", string parentName = "")
    {
        string selfName = t.name.Replace(" ", "");
        var result = new List<PrefabChildInfo>
        {
            new PrefabChildInfo
            {
                SelfName = selfName,
                childPath = transformName,
                selfObj = t.gameObject,
                componentInfos = new List<ComponentInfo>()
            }
        };

        if (t.childCount == 0)
        {
            return result;
        }

        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            var childName = string.IsNullOrEmpty(transformName) ? child.name : $"{transformName}/{child.name}";
            var list = GetList(child, childName, selfName);
            result.AddRange(list);
        }

        return result;
    }

    public void Generate()
    {
        var generator = ScriptGenerator.GetScriptTemplateSetting();
        if (!generator) return;
        this.ViewComponents();
        try
        {
            if (this.uiFormOperation == null)
            {
                Debug.LogError($"预制体缺少组件 UIFormOperation.");
                return;
            }
            generator.Generate(this);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void GenerateItem()
    {
        var generator = ScriptGenerator.GetScriptTemplateSetting();
        if (!generator) return;
        this.ViewComponents();
        try
        {
            if (this.uiFormOperation != null)
            {
                Debug.LogError($"当前预制体为 Form 而非 Item");
                return;
            }
            generator.GenerateItem(this);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    void OnTitleBarGUI()
    {
        var label = isReadonly ? "Show All" : "Show Used";
        if (GUILayout.Button(label))
        {
            ToggleReadonly();
        }
    }

    void OnPrefabPathChanged()
    {
        prefabGUID = AssetDatabase.AssetPathToGUID(PrefabPath);
    }

    void OnInspectorInit()
    {
        var isHasGUID = !string.IsNullOrEmpty(prefabGUID);
        var isHasPath = !string.IsNullOrEmpty(PrefabPath) && File.Exists(PrefabPath);

        if (isHasPath && !isHasGUID)
        {
            RefreshGUIDByPath();
        }

        if (!isHasPath && isHasGUID)
        {
            RefreshPathByGUID();
        }
        
        ViewComponents();
    }

    void RefreshGUIDByPath()
    {
        Debug.LogError($"提示 {name}缺少GUID,使用资源{PrefabPath}的GUID进行了覆盖");
        prefabGUID = AssetDatabase.AssetPathToGUID(PrefabPath);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    void RefreshPathByGUID()
    {
        var path = AssetDatabase.GUIDToAssetPath(prefabGUID);
        if (!string.IsNullOrEmpty(path))
        {
            Debug.LogError($"提示 {name}缺少路径,使用资源:{prefabGUID}的路径进行了覆盖");
            var l = "Assets/BundleResource/".Length;
            prefabPath = path.Substring(l, path.Length - l);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    void OnPathChanged()
    {
        if (!string.IsNullOrEmpty(PrefabPath) && File.Exists(PrefabPath))
        {
            RefreshGUIDByPath();
        }
        else
        {
            prefabGUID = string.Empty;
            Debug.Log($"{name} guid is empty");
        }
    }
#endif
}