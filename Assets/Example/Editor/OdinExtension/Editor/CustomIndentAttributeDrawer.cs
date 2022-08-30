using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEditor;
using UnityEngine;

public class CustomIndentAttributeDrawer : OdinAttributeDrawer<CustomIndentAttribute>
{
    private ValueResolver<int> getIndentResolver;

    protected override void Initialize()
    {
        getIndentResolver = ValueResolver.Get<int>(Property, Attribute.getIndent);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var indent = getIndentResolver.GetValue();
        EditorGUI.indentLevel = indent;
        CallNextDrawer(label);
    }
}