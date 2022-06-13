using System;
using System.Threading.Tasks;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace FuXi.Editor
{
    public class FxEditorAsset : FxAsset
    {
        internal static FxEditorAsset CreateEditorAsset(string path, Type type, bool immediate, Action<FxAsset> callback)
        { return new FxEditorAsset(path, type, immediate, callback); }
        
        FxEditorAsset(string path, Type type, bool loadImmediate, Action<FxAsset> callback) : base(path, type, loadImmediate, callback) { }
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