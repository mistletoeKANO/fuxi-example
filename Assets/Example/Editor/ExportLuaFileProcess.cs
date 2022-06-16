
using UnityEngine;

public class ExportLuaFileProcess : FuXi.Editor.IBuildBundlePreprocess
{
    public void BuildBundlePre()
    {
        Debug.Log("ExportLuaFileProcess pre");
    }

    public void BuildBundlePost()
    {
        Debug.Log("ExportLuaFileProcess post");
    }
}

public class ExportLuaFileProcess2 : FuXi.Editor.IBuildBundlePreprocess
{
    public void BuildBundlePre()
    {
        Debug.Log("ExportLuaFileProcess2 pre");
    }

    public void BuildBundlePost()
    {
        Debug.Log("ExportLuaFileProcess2 post");
    }
}