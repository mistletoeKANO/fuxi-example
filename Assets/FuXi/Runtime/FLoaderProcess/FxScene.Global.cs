using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace FuXi
{
    public partial class FxScene
    {
        private static FxScene Main;
        internal static Func<string, bool, bool, FxScene> FxSceneCreate;

        internal static FxScene CreateScene(string path, bool addition, bool immediate)
        { return new FxScene(path, addition, immediate); }

        public static FxScene LoadScene(string path, bool additive = false)
        {
            var res = FxSceneCreate.Invoke(path, additive, true).Execute();
            var scene = (FxScene) res.Result;
            RefreshRef(scene);
            return scene;
        }
        
        public static async Task<FxScene> LoadSceneAsync(string path, bool additive = false)
        {
            var res = await FxSceneCreate.Invoke(path, additive, false).Execute();
            var scene = (FxScene) res;
            RefreshRef(scene);
            return scene;
        }

        public static FxScene LoadSceneCo(string path, bool additive = false)
        {
            var scene = FxSceneCreate.Invoke(path, additive, false);
            scene.Execute();
            RefreshRef(scene);
            return scene;
        }

        private static void RefreshRef(FxScene fxScene)
        {
            if (fxScene.m_LoadMode == LoadSceneMode.Additive)
                Main?.m_SubScenes.Add(fxScene);
            else
            {
                Main?.Release();
                Main = fxScene;
            }
        }
    }
}