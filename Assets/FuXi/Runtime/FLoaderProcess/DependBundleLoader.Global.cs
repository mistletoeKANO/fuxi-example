using System.Collections.Generic;

namespace FuXi
{
    public partial class DependBundleLoader
    {
        private static readonly Dictionary<string, DependBundleLoader> UsedBundleDic = new Dictionary<string, DependBundleLoader>();
        private static readonly Queue<DependBundleLoader> UnUsedBundle = new Queue<DependBundleLoader>();

        internal static void UpdateUnUsed()
        {
            if (UnUsedBundle.Count == 0) return;
            while (UnUsedBundle.Count > 0)
            {
                if (AssetPolling.IsTimeOut) break;
                
                var releaseBundle = UnUsedBundle.Dequeue();
                if (releaseBundle.assetBundle == null) return;
                releaseBundle.assetBundle.Unload(true);
            }
        }

        internal static DependBundleLoader ReferenceBundle(BundleManifest manifest)
        {
            if (!UsedBundleDic.TryGetValue(manifest.BundleHashName, out var bundleLoader))
            {
                bundleLoader = new DependBundleLoader(manifest);
                UsedBundleDic.Add(manifest.BundleHashName, bundleLoader);
            }
            bundleLoader.AddReference();
            return bundleLoader;
        }
    }
}