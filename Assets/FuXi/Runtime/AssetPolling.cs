using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace FuXi
{
    public class AssetPolling : MonoBehaviour
    {
        private static readonly float timeSlice = 0.05f; // s
        private static float lastCheckTime = 0;
        internal static bool IsTimeOut
        {
            get
            {
                var curTime = Time.realtimeSinceStartup;
                if (curTime - lastCheckTime >= timeSlice)
                {
                    lastCheckTime = curTime;
                    return true;
                }
                lastCheckTime = curTime;
                return false;
            }
        }

        private void Update()
        {
            FxAsyncTask.UpdateProcess();
            DependBundleLoader.UpdateUnUsed();
        }

        private void OnApplicationQuit()
        {
            FxAsyncTask.ProcessQuit();
        }
    }
}