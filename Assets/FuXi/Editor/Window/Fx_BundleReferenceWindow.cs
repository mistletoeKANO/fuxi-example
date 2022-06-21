using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    internal class Fx_BundleReferenceWindow : EditorWindow, IHasCustomMenu
    {
        private Toolbar m_ToolBar;
        private VisualElement m_MainView;
        private VisualElement m_Footer;

        private Fx_BundleView m_BundleView;
        private Fx_AssetView m_AssetView;
        private const int ColumnHeight = 20;

        private ViewType m_ViewType = ViewType.BundleView;
        
        private void OnEnable()
        {
            this.InitWindow();
            this.InitGUIData();
            this.DrawGUI();
        }
        private void InitWindow() { this.titleContent = Fx_Style.Title; }

        private void InitGUIData()
        {
            this.m_BundleView = new Fx_BundleView(ColumnHeight);
            this.m_AssetView = new Fx_AssetView(ColumnHeight);
        }

        private void DrawGUI()
        {
            var style = Resources.Load<StyleSheet>("Uss/Fx_BundleReferenceWindow");
            this.rootVisualElement.styleSheets.Add(style);
            
            this.m_ToolBar = new Toolbar();
            this.DoToolbar();
            this.rootVisualElement.Add(this.m_ToolBar);

            this.m_MainView = new BindableElement();
            this.DoMain();
            this.rootVisualElement.Add(this.m_MainView);

            this.m_Footer = new VisualElement();
            this.DoFooter();
            this.rootVisualElement.Add(this.m_Footer);
        }

        private void DoToolbar()
        {
            this.m_ToolBar.AddToClassList(Fx_Style.CName_BF_Toolbar);

            var menu = new UnityEditor.UIElements.ToolbarMenu {text = this.m_ViewType.ToString()};
            menu.menu.AppendAction(ViewType.AssetView.ToString(), action =>
            {
                this.m_ViewType = ViewType.AssetView;
                menu.text = this.m_ViewType.ToString();
            });
            menu.menu.AppendAction(ViewType.BundleView.ToString(), action =>
            {
                this.m_ViewType = ViewType.BundleView;
                menu.text = this.m_ViewType.ToString();
            });
            menu.AddToClassList(Fx_Style.CName_BF_Toolbar_Enum);
            this.m_ToolBar.Add(menu);
        }

        private void DoMain()
        {
            this.m_MainView.AddToClassList(Fx_Style.CName_BF_MainView);

            var background = new Image();
            background.AddToClassList(Fx_Style.CName_BF_MainView_BG);
            this.m_MainView.Add(background);
            
            var view = new IMGUIContainer(this.OnDrawGUI);
            view.AddToClassList(Fx_Style.CName_BF_MainView_ScrollView);
            this.m_MainView.Add(view);
        }

        private void OnDrawGUI()
        {
            if (this.m_BundleView == null || this.m_AssetView == null) this.InitGUIData();
            GUILayout.Space(0);
            var windowRect = GUILayoutUtility.GetLastRect();
            windowRect.width = this.position.width;
            windowRect.height = this.position.height;
            if (this.m_ViewType == ViewType.BundleView)
                this.m_BundleView.OnGUI(windowRect);
            else if (this.m_ViewType == ViewType.AssetView)
                this.m_AssetView.OnGUI(windowRect);
        }

        

        private void DoFooter()
        {
            this.m_Footer.AddToClassList(Fx_Style.CName_BF_Footer);
            this.m_Footer.Add(new Label("fx"));
        }

        public void AddItemsToMenu(GenericMenu menu) { }

        private void OnDestroy()
        {
            
        }
    }

    internal enum ViewType
    {
        BundleView,
        AssetView,
    }
}