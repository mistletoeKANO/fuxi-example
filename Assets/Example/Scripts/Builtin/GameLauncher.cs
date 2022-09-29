using FuXi;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Builtin
{
    public class GameLauncher : MonoBehaviour
    {
        public RuntimeMode RuntimeMode;

#if UNITY_EDITOR
        private string VersionFile = "FuXiAssetWindow";
#else
        private string VersionFile = "FuXiAssetAndroid";
#endif

        private void Awake()
        {
            System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Debugger.LogError(e.ExceptionObject.ToString());
            };
        }
        
        private void InitFxLOGInformation()
        {
            //FuXi.FX_LOG_CONTROL.LogLevel = FX_LOG_TYPE.ERROR;
        }

        private async void Start()
        {
            InitFxLOGInformation();
            
            await FuXiManager.FxLauncherAsync(VersionFile, "http://192.168.1.2/Windows/", RuntimeMode, typeof(FxEncryptOffset));

            await new CheckUpdater().Start();
            
            AppStart.Start();
        }
    }
}
