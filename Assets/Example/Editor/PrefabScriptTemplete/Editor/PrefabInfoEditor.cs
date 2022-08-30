using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabInfo))]
public class PrefabInfoEditor : OdinEditor
{
    protected override void OnHeaderGUI()
    {
        GUILayout.Space(10f);
    }
}