using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FuXi
{
    public partial class FxScene : FxAsyncTask
    {
        private enum LoadSceneSteps
        {
            LoadBundle,
            LoadScene,
        }
        internal readonly string m_ScenePath;
        internal readonly LoadSceneMode m_LoadMode;
        internal readonly bool m_Immediate;
        internal AsyncOperation m_Operation;
        
        private readonly List<FxScene> m_SubScenes = new List<FxScene>();
        private BundleLoader m_BundleLoader;

        private LoadSceneSteps m_LoadStep;

        internal FxScene(string path, bool additive, bool immediate)
        {
            this.m_ScenePath = path;
            this.m_LoadMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.m_Immediate = immediate;
            this.m_BundleLoader = new BundleLoader();
        }
        
        internal override Task<FxAsyncTask> Execute()
        {
            base.Execute();
            if (FxManager.RuntimeMode == RuntimeMode.Editor) return null;
            if (!FxManager.ManifestVC.TryGetAssetManifest(this.m_ScenePath, out var manifest))
            {
                this.tcs.SetResult(this);
                this.isDone = true;
                return this.tcs.Task;
            }
            this.m_BundleLoader.StartLoad(manifest, this.m_Immediate);
            if (this.m_Immediate)
            {
                if (this.m_BundleLoader.mainLoader.assetBundle != null)
                    SceneManager.LoadScene(this.m_ScenePath, this.m_LoadMode);
                RefreshRef(this);
                this.tcs.SetResult(this);
                this.isDone = true;
            }
            this.m_LoadStep = LoadSceneSteps.LoadBundle;
            return this.tcs.Task;
        }

        protected override void Update()
        {
            if (this.isDone) return;
            switch (this.m_LoadStep)
            {
                case LoadSceneSteps.LoadBundle:
                    if (!this.m_BundleLoader.isDone)
                    {
                        this.m_BundleLoader.Update();
                        return;
                    }
                    this.m_Operation = SceneManager.LoadSceneAsync(this.m_ScenePath, this.m_LoadMode);
                    this.m_LoadStep = LoadSceneSteps.LoadScene;
                    break;
                case LoadSceneSteps.LoadScene:
                    if (this.m_Operation.allowSceneActivation)
                        if (!this.m_Operation.isDone) return;
                    else
                        if (this.m_Operation.progress < 0.9f) return;
                    RefreshRef(this);
                    this.isDone = true;
                    this.tcs.SetResult(this);
                    break;
            }
        }

        private void Release()
        {
            this.m_BundleLoader.Release();
            foreach (var fxScene in this.m_SubScenes)
            {
                fxScene.Release();
            }
            SceneManager.UnloadSceneAsync(this.m_ScenePath);
            this.m_SubScenes.Clear();
            this.Dispose();
        }

        protected override void Dispose()
        {
            this.m_BundleLoader.Dispose();
            this.m_BundleLoader = null;
        }
    }
}