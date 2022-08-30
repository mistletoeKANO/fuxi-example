using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class ToggleColorButtonAttributeDrawer : OdinAttributeDrawer<ToggleColorButtonAttribute>
{
    private ValueResolver<string> enableStringResolver;
    private ValueResolver<string> disableStringResolver;
    private ValueResolver<Color> enableColorResolver;
    private ValueResolver<Color> disableColorResolver;

    protected override void Initialize()
    {
        enableStringResolver = ValueResolver.Get<string>(Property, Attribute.enableString);
        disableStringResolver = ValueResolver.Get<string>(Property, Attribute.disableString);
        enableColorResolver = string.IsNullOrEmpty(Attribute.enableColor) ? null : ValueResolver.Get<Color>(Property, Attribute.enableColor);
        disableColorResolver = string.IsNullOrEmpty(Attribute.disableColor) ? null : ValueResolver.Get<Color>(Property, Attribute.disableColor);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUILayout.BeginHorizontal();
        var isToggleEnable = (bool) Property.ValueEntry.WeakSmartValue;
        var buttonLabel = isToggleEnable ? enableStringResolver.GetValue() : disableStringResolver.GetValue();
        var color = isToggleEnable
            ? enableColorResolver?.GetValue() ?? Color.white
            : disableColorResolver?.GetValue() ?? Color.gray;
        GUIHelper.PushColor(color);
        if (GUILayout.Button(buttonLabel))
        {
            Property.ValueEntry.WeakSmartValue = !isToggleEnable;
        }
        GUIHelper.PopColor();
        GUILayout.EndHorizontal();
    }
}