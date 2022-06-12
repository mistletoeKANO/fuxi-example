using System;
using System.Threading.Tasks;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    public class FxEditorAsset : FxAsset
    {
        internal static FxEditorAsset CreateEditorAsset(string path, Type type, bool immediate)
        { return new FxEditorAsset(path, type, immediate); }
        
        FxEditorAsset(string path, Type type, bool loadImmediate) : base(path, type, loadImmediate) { }
        internal override Task<FxAsyncTask> Execute()
        {
            base.Execute();
            if (FxManager.ManifestVC.TryGetAssetManifest(this.m_FilePath, out var _))
                this.asset = AssetDatabase.LoadAssetAtPath(this.m_FilePath, this.m_Type);
            this.tcs.SetResult(this);
            this.isDone = true;
            return this.tcs.Task;
        }
    }
}