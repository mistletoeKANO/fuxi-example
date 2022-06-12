using UnityEngine;

namespace FuXi.Editor
{
    public static class Fx_Style
    {
        public static readonly Texture2D Fx_Asset = Resources.Load<Texture2D>("Gizmos/Fx_Asset");
        public static readonly Texture2D Fx_About = Resources.Load<Texture2D>("Gizmos/Fx_About");
        public static readonly Texture2D Fx_PathMenu = Resources.Load<Texture2D>("Gizmos/Fx_PathMenu");
        public static readonly Texture2D Fx_AssetBlack = Resources.Load<Texture2D>("Gizmos/Fx_Asset Black");
        public static readonly Texture2D Fx_AssetPackage = Resources.Load<Texture2D>("Gizmos/Fx_AssetPackage");

        public static readonly string Fx_BuildAsset_Uss = "Uss/Fx_BuildAsset";
        public static readonly string Fx_BuildAsset_Root_Class = "fx-buildAsset-root";
        public static readonly string Fx_BuildAsset_Main_Class = "fx-buildAsset-main";
        public static readonly string Fx_BuildAsset_Foot_Class = "fx-buildAsset-foot";
    }
}