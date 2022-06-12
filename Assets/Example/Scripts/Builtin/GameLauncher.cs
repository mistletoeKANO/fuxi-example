using FuXi;
using Game.HotFix;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Builtin
{
    public class GameLauncher : MonoBehaviour
    {
        public RuntimeMode RuntimeMode;
    
        private async void Start()
        {
            await FxManager.FxLauncherAsync(RuntimeMode);
            
            SceneManager.Instance.Init();
            UIManager.Instance.Init();
            
            SceneManager.Instance.SwitchScene<CheckUpdateScene>(SceneConfigs.CheckUpdateScene);
        }
    }
}
