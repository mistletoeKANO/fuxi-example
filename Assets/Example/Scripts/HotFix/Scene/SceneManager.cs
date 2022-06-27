using System;
using FuXi;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.HotFix
{
    public class SceneManager : Singleton<SceneManager>
    {
        public Camera MainCamera;
        private SceneHandle m_Current;

        public void Init()
        {
            Object.DontDestroyOnLoad(Camera.main);
            this.MainCamera = Camera.main;
        }
        public async void SwitchScene<T>(string scenePath, bool additive = false) where T : SceneHandle
        {
            if (this.m_Current != null && this.m_Current.GetType() == typeof(T)) return;
            await FxScene.LoadSceneAsync(scenePath, additive, f=>{Debug.Log(f);});
            this.m_Current?.Exist();
            this.m_Current = Activator.CreateInstance<T>();
            this.m_Current.Enter();
        }
    }
}