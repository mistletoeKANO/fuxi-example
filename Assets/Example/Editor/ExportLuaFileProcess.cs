
using UnityEngine;

public class ExportLuaFileProcess : FuXi.Editor.IBuildBundlePreprocess
{
    public void BuildBundlePre()
    {
        Debug.Log("ExportLuaFileProcess pre");
    }

    public void BuildBundlePost(System.Collections.Generic.List<string> diffFiles)
    {
        Debug.Log($"ExportLuaFileProcess post: {diffFiles.Count}");
    }
}