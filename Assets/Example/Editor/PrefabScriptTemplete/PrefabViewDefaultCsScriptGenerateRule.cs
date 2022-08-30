using System;
using System.IO;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 默认的CSharp代码生成规则。
/// </summary>
[CreateAssetMenu(fileName = "PrefabCsRule", menuName = "ScriptGenerateRule/PrefabCsRule")]
public class PrefabViewDefaultCsScriptGenerateRule : PrefabViewScriptGenerateRule
{
    [TextArea(3, 100)]
    [PropertyOrder(2)]
    [Space]
    [LabelText("View模板")]
    public string bindTemplate;
    
    [TextArea(3, 100)]
    [PropertyOrder(2)]
    [LabelText("ItemBinder模板")]
    public string itemBindTemplate;
    
    [PropertyOrder(2)]
    [LabelText("View组件属性"), LabelWidth(120)]
    [TextArea(3, 100)]
    public string bindComponentField;
    
    [TextArea(3, 100)]
    [PropertyOrder(2)]
    [LabelText("Flag查找物体"), LabelWidth(120)]
    public string BindFlag;
    
    [PropertyOrder(2)]
    [LabelText("view组件"), LabelWidth(120)]
    public string bindComponent;
    
    [PropertyOrder(2)]
    [LabelText("view Item属性"), LabelWidth(120)]
    public string bindItem;
    
    [TextArea(3, 100)]
    [PropertyOrder(2)]
    [LabelText("View模板")]
    public string viewTemplate;
    
    private const string PrefabName = "#PrefabName#";
    private const string ComponentField = "#ComponentField#";
    private const string ComponentBinder = "#ComponentBinder#";
    private const string DisposeComponent = "#DisposeComponent#";
    
    private const string ComponentType = "#ComponentType#";
    private const string FlagName = "#FlagName#";
    private const string FieldName = "#FieldName#";
    private const string PropertyName = "#PropertyName#";
    private const string ComponentPath = "#ComponentPath#";
    
    private const string PrefabPath = "#PrefabPath#";
    
    [NonSerialized]
    private string resultRootPath = "Assets/Example/Scripts/HotFix/UI";
    private string viewResultDir;
    private string presenterResultDir;

    public override void Generate(PrefabInfo prefabInfo)
    {
        try
        {
            viewResultDir = $"{resultRootPath}/Views";
            presenterResultDir = $"{resultRootPath}/Presenters";

            if (!Directory.Exists(viewResultDir))
            {
                Directory.CreateDirectory(viewResultDir);
            }

            if (!Directory.Exists(presenterResultDir))
            {
                Directory.CreateDirectory(presenterResultDir);
            }
            GenerateView(prefabInfo);
            GeneratePresenter(prefabInfo);
            // GenerateModel(prefabInfo);
            Debug.Log($"预制体：{prefabInfo.PrefabName} 生成脚本完成");
        }
        catch (Exception e)
        {
            throw new Exception($"generate ui view failure with error: {e.Message}");
        }
    }

    private string ReplaceField(PrefabChildInfo childInfo, string type, string simpleName, bool isArray = false)
    {
        var fieldName = childInfo.SelfName
            .Replace(" ", "")
            .Replace("(", "")
            .Replace(")", "");
        var lowerSimpleName = string.Empty;
        if (!string.IsNullOrEmpty(simpleName))
        {
            lowerSimpleName = $"{simpleName.First().ToString().ToLower()}{simpleName.Substring(1)}";
            var viewField = bindComponentField
                .Replace(ComponentType, isArray ? $"{type}[]" : $"{type}")
                .Replace(FieldName, $"{lowerSimpleName}{fieldName}")
                .Replace(PropertyName, $"{simpleName}{fieldName}");
            return $"{viewField}";
        }
        else
        {
            lowerSimpleName = $"{fieldName.First().ToString().ToLower()}{fieldName.Substring(1)}";
            var viewField = bindComponentField
                .Replace(ComponentType, isArray ? $"{type}[]" : $"{type}")
                .Replace(FieldName, $"{lowerSimpleName}")
                .Replace(PropertyName, $"{fieldName}");
            return $"{viewField}";
        }
    }


    private string ReplaceBindFlag(PrefabChildInfo childInfo, int index)
    {
        var bindFlag = BindFlag
            .Replace(ComponentPath, childInfo.childPath)
            .Replace(FlagName, $"f_{index}");
        return $"\t\t{bindFlag}";
    }
    private string ReplaceBinder(PrefabChildInfo childInfo, string type, string simpleName, int index)
    {
        var fieldName = childInfo.SelfName
            .Replace(" ", "")
            .Replace("(", "")
            .Replace(")", "");
        var lowerSimpleName = $"{simpleName.First().ToString().ToLower()}{simpleName.Substring(1)}";
        var bindCmp = bindComponent
            .Replace(FlagName, $"f_{index}")
            .Replace(FieldName, $"{lowerSimpleName}{fieldName}")
            .Replace(ComponentType, $"{type}")
            .Replace(PropertyName, $"{simpleName}{fieldName}");
        return $"\t\t\t{bindCmp}";
    }
    
    private string ReplaceItemView(PrefabChildInfo childInfo, string type, int index)
    {
        var fieldName = childInfo.SelfName
            .Replace(" ", "")
            .Replace("(", "")
            .Replace(")", "");
        var lowerSimpleName = $"{fieldName.First().ToString().ToLower()}{fieldName.Substring(1)}";
        var bindField = bindItem
            .Replace(FlagName, $"f_{index}")
            .Replace(PrefabName, type)
            .Replace(FieldName, $"{lowerSimpleName}")
            .Replace(PropertyName, $"{type}{fieldName}");
        return $"\t\t\t{bindField}";
    }

    private void GenerateView(PrefabInfo prefabInfo)
    {
        var file = $"{viewResultDir}/{prefabInfo.PrefabName}View.cs"; //view 每次生成更新内容
        string bindContent = (string) bindTemplate.Clone();
        GenerateViewInternal(prefabInfo, bindContent, file);
    }

    private void GenerateViewInternal(PrefabInfo prefabInfo, string bindContent, string path)
    {
        bindContent = bindContent.Replace(PrefabName, prefabInfo.PrefabName);

        StringBuilder FieldSB = new StringBuilder();
        StringBuilder ComponentBindSB = new StringBuilder();
        StringBuilder DisposeSB = new StringBuilder();
        string groupPath = String.Empty;
        string prefabPath = String.Empty;

        for (int i = 0; i < prefabInfo.childrenInfos.Count; i++)
        {
            var childInfo = prefabInfo.childrenInfos[i];
            if (childInfo.Contains("UIGroup"))
            {
                groupPath = childInfo.childPath;
                ComponentBindSB.AppendLine(ReplaceBindFlag(childInfo, i));
                FieldSB.AppendLine(ReplaceField(childInfo, $"{typeof(UIGroup)}", "Group"));
                ComponentBindSB.AppendLine(ReplaceBinder(childInfo, $"{typeof(UIGroup)}","Group", i));
                ComponentBindSB.AppendLine("\t\t}");
                continue;
            }
            if (!string.IsNullOrEmpty(groupPath) && childInfo.childPath.Contains(groupPath) && childInfo.childPath.Contains("/")) continue;
            groupPath = string.Empty;

            if (PrefabUtility.IsPartOfPrefabInstance(childInfo.selfObj))
            {
                if (!string.IsNullOrEmpty(prefabPath)) continue;
                if (childInfo.Contains("UIIgnore"))
                    prefabPath = String.Empty;
                else
                {
                    prefabPath = childInfo.childPath;
                    var o = PrefabUtility.GetCorrespondingObjectFromOriginalSource(childInfo.selfObj);
                    ComponentBindSB.AppendLine(ReplaceBindFlag(childInfo, i));
                    FieldSB.AppendLine(ReplaceField(childInfo, o.name, ""));
                    ComponentBindSB.AppendLine(ReplaceItemView(childInfo, o.name, i));
                    ComponentBindSB.AppendLine("\t\t}");
                    continue;
                }
            }else prefabPath = String.Empty;

            if (childInfo.Enable())
                ComponentBindSB.AppendLine(ReplaceBindFlag(childInfo, i));

            foreach (var component in childInfo.componentInfos)
            {
                if (!component.enable) continue;
                FieldSB.AppendLine(ReplaceField(childInfo, component.type.FullName, component.SimpleName));
                ComponentBindSB.AppendLine(ReplaceBinder(childInfo, component.type.FullName, component.SimpleName, i));
            }

            if (childInfo.Enable())
                ComponentBindSB.AppendLine("\t\t}");
        }
        bindContent = bindContent.Replace(ComponentField, FieldSB.ToString())
            .Replace(PrefabPath, prefabInfo.prefabPath)
            .Replace(ComponentBinder, ComponentBindSB.ToString())
            .Replace(DisposeComponent, DisposeSB.ToString());
        File.WriteAllText(path, bindContent, Encoding.UTF8);
        Debug.Log($"生成脚本:{path}");
    }

    private void GeneratePresenter(PrefabInfo prefabInfo)
    {
        var file = $"{presenterResultDir}/{prefabInfo.PrefabName}.cs"; //view 只生成一次
        if (File.Exists(file)) return;
        var viewContent = (string) viewTemplate.Clone();

        viewContent = viewContent
            .Replace(PrefabPath, prefabInfo.prefabPath)
            .Replace(PrefabName, prefabInfo.PrefabName);
        File.WriteAllText(file, viewContent, Encoding.UTF8);
        Debug.Log($"生成脚本：{file}");
    }
    
    public override void GenerateItemView(PrefabInfo prefabInfo)
    {
        try
        {
            var dir = $"{resultRootPath}/Views/EUIItem";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            
            var file = $"{dir}/{prefabInfo.PrefabName}.cs"; //view 每次生成更新内容
            string bindContent = (string) itemBindTemplate.Clone();
            GenerateViewInternal(prefabInfo, bindContent, file);
            Debug.Log($"生成Item脚本 {prefabInfo.PrefabName} 完成!");
        }
        catch (Exception e)
        {
            throw new Exception($"generate ui view failure with error: {e.Message}");
        }
    }
}