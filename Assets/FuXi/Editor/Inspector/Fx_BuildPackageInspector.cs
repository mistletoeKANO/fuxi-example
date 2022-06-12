using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    [CustomEditor(typeof(Fx_BuildPackage), true)]
    [CanEditMultipleObjects]
    public class Fx_BuildPackageInspector : UnityEditor.Editor
    {
        private VisualElement m_Root = null;
        static class Style
        {
            public static readonly GUIContent PackageObjects = EditorGUIUtility.TrTextContent("分包资产");
        }
        
        SerializedProperty m_PackageObjects;

        private void OnEnable()
        {
            this.m_PackageObjects = serializedObject.FindProperty("PackageObjects");
        }

        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            var main_style = Resources.Load<StyleSheet>(Fx_Style.Fx_BuildAsset_Uss);
            if (main_style != null)
            {
                this.m_Root.styleSheets.Add(main_style);
            }
            this.m_Root.AddToClassList(Fx_Style.Fx_BuildAsset_Root_Class);
            
            IMGUIContainer imguiContainer = new IMGUIContainer(this.OnGUI);
            this.m_Root.Add(imguiContainer);
            imguiContainer.AddToClassList(Fx_Style.Fx_BuildAsset_Main_Class);

            System.Threading.Tasks.Task.Factory.StartNew(this.ResetParentStyle);
            return this.m_Root;
        }

        private void ResetParentStyle()
        {
            this.m_Root.parent.RemoveFromClassList(InspectorElement.uIEInspectorVariantUssClassName);
            this.m_Root.parent.style.paddingLeft = 0;
            this.m_Root.parent.style.paddingRight = 0;
            this.m_Root.parent.style.paddingTop = 0;
            this.m_Root.parent.style.paddingBottom = 0;
        }

        private void OnGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_PackageObjects, Style.PackageObjects);
            serializedObject.ApplyModifiedProperties();
        }
    }
}