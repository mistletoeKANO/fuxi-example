using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class ColorIfAttributeDrawer : OdinAttributeDrawer<ColorIfAttribute>
{
    private ValueResolver<Color> colorResolver;

    private ValueResolver<bool> conditionResolver;

    protected override void Initialize()
    {
        colorResolver = ValueResolver.Get<Color>(Property, Attribute.Color);
        conditionResolver = ValueResolver.Get<bool>(Property, Attribute.Condition);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var condition = conditionResolver.GetValue();
        if (condition)
        {
            var color = colorResolver.GetValue();
            GUIHelper.PushColor(color);
        }

        CallNextDrawer(label);

        if (condition)
        {
            GUIHelper.PopColor();
        }
    }
}