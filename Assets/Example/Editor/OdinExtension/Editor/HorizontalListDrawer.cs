using System.Collections;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class HorizontalListDrawer<T> : OdinAttributeDrawer<HorizontalListAttribute, T> where T : IList
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUILayout.BeginHorizontal();
        for (int i = 0; i < Property.Children.Count; i++)
        {
            GUILayout.BeginVertical();
            // Probably set label width and stuff so things display properly
            Property.Children[i].Draw(null);
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }
}