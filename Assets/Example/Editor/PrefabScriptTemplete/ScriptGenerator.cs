using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ScriptGenerator : ScriptableObject
{
    const string scriptTemplateAssetPath = "Assets/Example/Configs/UITemplate/ScriptTemplate.asset";
    
    [Title("Prefab View Generate Rule")]
    [HideLabel]
    public PrefabViewScriptGenerateRule prefabViewScriptGenerateRule;

    public void Generate(PrefabInfo prefabInfo)
    {
        if (null == prefabViewScriptGenerateRule)
            throw new Exception($"no rule config in {scriptTemplateAssetPath}");
        prefabViewScriptGenerateRule.Generate(prefabInfo);
    }
    
    public void GenerateItem(PrefabInfo prefabInfo)
    {
        if (null == prefabViewScriptGenerateRule)
            throw new Exception($"no rule config in {scriptTemplateAssetPath}");
        prefabViewScriptGenerateRule.GenerateItemView(prefabInfo);
    }

    public static ScriptGenerator GetScriptTemplateSetting()
    {
        return GetOrCreateSetting<ScriptGenerator>(scriptTemplateAssetPath);
    }
    
    /// <summary>
    /// 从某个路径加载或新建一个配置。
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static T GetOrCreateSetting<T>(string path) where T : ScriptableObject
    {
#if UNITY_EDITOR
        var settings = AssetDatabase.LoadAssetAtPath<T>(path);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
        }

        return settings;
#endif
    }
}