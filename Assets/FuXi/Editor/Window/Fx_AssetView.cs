using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    public class Fx_AssetView : Fx_BaseView
    {
        internal Fx_AssetView(int columnHeight)
        {
            this.columnHeight = columnHeight;
            this.m_Columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent  = new GUIContent("-"),
                    headerTextAlignment = TextAlignment.Center,
                    canSort = false,
                    width = 40,
                    maxWidth = 60
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Asset Name"),
                    minWidth = 240,
                    width = 320,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Ref Count"),
                    width = 80,
                    maxWidth = 120,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Ref Bundle ID"),
                    width = 80,
                    maxWidth = 120,
                },
            };
            this.m_MultiColumnHeaderState = new MultiColumnHeaderState(this.m_Columns);
            this.m_MultiColumnHeader = new MultiColumnHeader(this.m_MultiColumnHeaderState);
            this.m_MultiColumnHeader.visibleColumnsChanged += header => { header.ResizeToFit(); };
            this.m_MultiColumnHeader.ResizeToFit();
        }

        internal void OnGUI(Rect windowRect)
        {
            Rect posRect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
            Rect viewRect = new Rect(windowRect) {xMax = this.m_Columns.Sum(c => c.width)};
            
            this.m_ColumnHeadWidth = Mathf.Max(posRect.width + this.m_ScrollPos.x, this.m_ColumnHeadWidth);
            Rect columnRect = new Rect(posRect) {width = this.m_ColumnHeadWidth, height = columnHeight};
            this.m_MultiColumnHeader.OnGUI(columnRect, this.m_ScrollPos.x);
            this.m_ScrollPos = GUI.BeginScrollView(posRect, this.m_ScrollPos, viewRect, false, false);
            int index = 0;
            int mMaxHeight = columnHeight;
            var assetDic = FuXi.FxAsset.AssetCache;
            foreach (var asset in assetDic)
            {
                Rect rowRect = new Rect(columnRect);
                int columnIndex = 0;
                Rect bgRect = new Rect(rowRect) {y = rowRect.y + mMaxHeight, height = columnHeight};
                EditorGUI.DrawRect(bgRect, index % 2 == 0 ? Fx_Style.C_ColumnDark : Fx_Style.C_ColumnLight);
                if (this.m_MultiColumnHeader.IsColumnVisible(columnIndex))
                {
                    int visibleColumnIndex = this.m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
                    Rect cRect = this.m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
                    cRect.y = rowRect.y + mMaxHeight;
                    GUI.Box(cRect, Fx_Style.PrefabIcon);
                }
                columnIndex++;
                if (this.m_MultiColumnHeader.IsColumnVisible(columnIndex))
                {
                    int visibleColumnIndex = this.m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
                    Rect cRect = this.m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
                    cRect.y = rowRect.y + mMaxHeight;
                    GUI.Label(cRect, asset.Key);
                }
                columnIndex++;
                if (this.m_MultiColumnHeader.IsColumnVisible(columnIndex))
                {
                    int visibleColumnIndex = this.m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
                    Rect cRect = this.m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
                    cRect.y = rowRect.y + mMaxHeight;
                    GUI.Label(cRect, asset.Value.fxReference.RefCount.ToString());
                }
                columnIndex++;
                if (this.m_MultiColumnHeader.IsColumnVisible(columnIndex))
                {
                    int visibleColumnIndex = this.m_MultiColumnHeader.GetVisibleColumnIndex(columnIndex);
                    Rect cRect = this.m_MultiColumnHeader.GetColumnRect(visibleColumnIndex);
                    cRect.y = rowRect.y + mMaxHeight;
                    GUI.Label(cRect, asset.Value.manifest.HoldBundle.ToString());
                }
                index++;
                mMaxHeight += columnHeight;
            }
            GUI.EndScrollView(true);
            this.m_MultiColumnHeader.OnGUI(columnRect, this.m_ScrollPos.x);
        }

        public override void Dispose() { }
    }
}