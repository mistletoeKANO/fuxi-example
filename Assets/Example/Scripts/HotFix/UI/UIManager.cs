using System.Collections.Generic;
using System.Threading.Tasks;
using FuXi;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.HotFix
{
    public class UIManager : Singleton<UIManager>
    {
        private readonly string m_UIRootPath = "Assets/Example/BundleResource/Prefabs/Game/UIRoot.prefab";
        public Camera UICamera;
        private Transform[] m_Layers = new Transform[6];
        private readonly Dictionary<string, Object> m_Windows = new Dictionary<string, Object>();
        public async void Init()
        {
            FxAsset fxAsset = await FxAsset.LoadAssetAsync<GameObject>(this.m_UIRootPath);
            var mUIRoot = (GameObject) Object.Instantiate(fxAsset.asset);
            Object.DontDestroyOnLoad(mUIRoot);
            ObjectReference reference = mUIRoot.GetComponent<ObjectReference>();
            
            for (int i = 0; i < 6; i++)
            {
                this.m_Layers[i] = reference.ObjectReferences[i].transform;
            }

            if (reference.ObjectReferences.Length < 6) return;
            
            var additionData = SceneManager.Instance.MainCamera.GetUniversalAdditionalCameraData();
            this.UICamera = reference.ObjectReferences[6].GetComponent<Camera>();
            additionData.cameraStack.Add(this.UICamera);
        }

        public async Task<GameObject> OpenWindow(string path, UILayer uiLayer)
        {
            FxAsset fxAsset = await FxAsset.LoadAssetAsync<GameObject>(path);
            var window = Object.Instantiate(fxAsset.asset, this.m_Layers[(int) uiLayer]);
            this.m_Windows.Add(path, window);
            return (GameObject) window;
        }

        public void CloseWindow(string path)
        {
            if (!this.m_Windows.TryGetValue(path, out var window)) return;
            
            Object.DestroyImmediate(window);
            this.m_Windows.Remove(path);
        }
    }
}