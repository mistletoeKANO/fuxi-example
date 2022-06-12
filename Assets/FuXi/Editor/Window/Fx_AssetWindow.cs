using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    internal class Fx_AssetWindow : EditorWindow, IHasCustomMenu
    {
        [SerializeField]
        internal Fx_BuildAsset mBuildAsset;

        private MultiColumnHeader mMultiColumnHeader;
        private MultiColumnHeaderState mMultiColumnHeaderState;
        private MultiColumnHeaderState.Column[] mColumns;

        private Vector2 mScrollPos;

        [SerializeField] private ObjectItem[] mItems = new[]
        {
            new ObjectItem{name = "Red", value = 0, color = Color.red},
            new ObjectItem{name = "Orange", value = 0.1f, color = new Color(1f, 0.41f, 0.05f)},
            new ObjectItem{name = "Yellow", value = 0.2f, color = Color.yellow},
            new ObjectItem{name = "Green", value = 0.3f, color = Color.green},
            new ObjectItem{name = "Cyan", value = 0.4f, color = Color.cyan},
            new ObjectItem{name = "Blue", value = 0.5f, color = Color.blue},
            new ObjectItem{name = "Purple", value = 0.6f, color = new Color(0.62f, 0.07f, 1f)},
        };

        internal void OpenWindow(Fx_BuildAsset buildAsset)
        {
            this.mBuildAsset = buildAsset;
            this.minSize = new Vector2(640, 120);
            this.OnSerializeFieldChanged();
        }

        private void Awake()
        {
            this.Init();
        }

        private void Init()
        {
            this.mColumns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    allowToggleVisibility = true,
                    autoResize = false,
                    minWidth = 80,
                    canSort = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    headerContent = new GUIContent("Name", "name"),
                    headerTextAlignment = TextAlignment.Left
                },
                new MultiColumnHeaderState.Column
                {
                    allowToggleVisibility = true,
                    autoResize = true,
                    minWidth = 240,
                    canSort = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    headerContent = new GUIContent("Value", "value"),
                    headerTextAlignment = TextAlignment.Left
                },
                new MultiColumnHeaderState.Column
                {
                    allowToggleVisibility = true,
                    autoResize = true,
                    minWidth = 160,
                    canSort = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    headerContent = new GUIContent("Color", "color"),
                    headerTextAlignment = TextAlignment.Left
                },
            };
            this.mMultiColumnHeaderState = new MultiColumnHeaderState(this.mColumns);
            this.mMultiColumnHeader = new MultiColumnHeader(this.mMultiColumnHeaderState);
            this.mMultiColumnHeader.visibleColumnsChanged += (header) => this.mMultiColumnHeader.ResizeToFit();
            this.mMultiColumnHeader.ResizeToFit();
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            if (this.mMultiColumnHeader == null) this.Init();
            
            GUILayout.FlexibleSpace();
            Rect windowRect = GUILayoutUtility.GetLastRect();
            windowRect.width = this.position.width;
            windowRect.height = this.position.height;
            
            Rect columnRect = new Rect(windowRect){height = 20};
            Rect positionRect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
            Rect viewRect = new Rect(windowRect){xMax = this.mColumns.Sum(column => column.width)};
            
            this.mScrollPos = GUI.BeginScrollView(positionRect, this.mScrollPos, viewRect, false, false);
            this.mMultiColumnHeader.OnGUI(columnRect, 0.0f);
            for (int i = 0; i < this.mItems.Length; i++)
            {
                ObjectItem item = this.mItems[i];
                
                Rect rowRect = new Rect(columnRect);
                rowRect.y += 20 * (i + 1);
                if (i%2==0)
                    EditorGUI.DrawRect(rowRect, Color.white * 0.1f);
                else
                    EditorGUI.DrawRect(rowRect, Color.white * 0.3f);
                
                int columnIndex = 0;
                if (this.mMultiColumnHeader.IsColumnVisible(columnIndex))
                {
                    int visibleIndex = this.mMultiColumnHeader.GetVisibleColumnIndex(columnIndex);
                    Rect columnFieldRect = this.mMultiColumnHeader.GetColumnRect(visibleIndex);
                    columnFieldRect.y = rowRect.y;
                    EditorGUI.LabelField(this.mMultiColumnHeader.GetCellRect(visibleIndex, columnFieldRect), item.name);
                }
            }
            
            GUI.EndScrollView(true);
        }

        private void OnDisable()
        {
            
        }

        private void OnDestroy()
        {
            
        }

        private void OnSerializeFieldChanged() => EditorUtility.SetDirty(this);

        public void AddItemsToMenu(GenericMenu menu) { }
        
        [Serializable]
        internal struct ObjectItem
        {
            public string name;
            public float value;
            public Color color;
        }
    }
}