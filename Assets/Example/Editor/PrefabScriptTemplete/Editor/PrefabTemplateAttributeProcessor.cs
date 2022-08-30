using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

public class PrefabTemplateAttributeProcessor : OdinAttributeProcessor<PrefabInfo>
{
    public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member,
        List<Attribute> attributes)
    {
        switch (member.Name)
        {
            case "prefabPath":
                attributes.Add(new OnInspectorInitAttribute("OnInspectorInit"));
                attributes.Add(new IndentAttribute(-1));
                attributes.Add(new OnValueChangedAttribute("OnPathChanged"));
                attributes.Add(new PropertyOrderAttribute(-2));
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new InlineButtonAttribute("PingPrefab", "Select"));
                attributes.Add(new FilePathAttribute
                {
                    Extensions = ".prefab",
                    AbsolutePath = false,
                    RequireExistingPath = true,
                    ParentFolder = "Assets/Example/BundleResource"
                });
                break;
            case "prefabGUID":
                attributes.Add(new DisplayAsStringAttribute());
                attributes.Add(new IndentAttribute(-1));
                attributes.Add(new HideLabelAttribute());
                break;
            case "childrenInfos":
                attributes.Add(new IndentAttribute(-1));
                attributes.Add(new LabelTextAttribute(" "));
                attributes.Add(new ValidateInputAttribute("IsValid"));
                attributes.Add(new InlinePropertyAttribute());
                attributes.Add(new LabelWidthAttribute(30));
                attributes.Add(new SearchableAttribute {Recursive = true, FuzzySearch = false});
                attributes.Add(new ListDrawerSettingsAttribute
                {
                    Expanded = true,
                    DraggableItems = false,
                    NumberOfItemsPerPage = 30,
                    IsReadOnly = true,
                    OnTitleBarGUI = "OnTitleBarGUI"
                });
                break;
            case "Generate":
                attributes.Add(new IndentAttribute(-1));
                attributes.Add(new PropertyOrderAttribute(-10));
                attributes.Add(new ButtonGroupAttribute("btn"));
                attributes.Add(new ButtonAttribute("Generate Form")
                {
                    ButtonHeight = 20
                });
                break;
            case "GenerateItem":
                attributes.Add(new PropertyOrderAttribute(-10));
                attributes.Add(new ButtonGroupAttribute("btn"));
                attributes.Add(new ButtonAttribute("Generate ItemView")
                {
                    ButtonHeight = 20
                });
                break;
        }
    }
}