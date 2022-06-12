using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuXi
{
    public partial class FxRawAsset
    {
        private static readonly Dictionary<string, FxRawAsset> RawAssetCache = new Dictionary<string, FxRawAsset>();
        internal static Func<string, FxRawAsset> FxRawAssetCreate;
        
        internal static FxRawAsset CreateRawAsset(string path)
        { return new FxRawAsset(path); }
        
        public static async Task<FxRawAsset> LoadRawAssetAsync(string path)
        {
            if (RawAssetCache.TryGetValue(path, out var rawAsset))
                return rawAsset;
            var res= await FxRawAssetCreate.Invoke(path).Execute();
            rawAsset = (FxRawAsset) res;
            RawAssetCache.Add(path, rawAsset);
            return rawAsset;
        }

        public static FxRawAsset LoadRawAssetCo(string path)
        {
            if (RawAssetCache.TryGetValue(path, out var rawAsset))
                return rawAsset;
            rawAsset = FxRawAssetCreate.Invoke(path);
            rawAsset.Execute();
            RawAssetCache.Add(path, rawAsset);
            return rawAsset;
        }

        public static void Release(string path)
        {
            if (!RawAssetCache.TryGetValue(path, out var rawAsset)) return;
            rawAsset.Dispose();
            RawAssetCache.Remove(path);
        }

        public static void ReleaseAll()
        {
            foreach (var rawAsset in RawAssetCache)
                rawAsset.Value.Dispose();
            RawAssetCache.Clear();
        }
    }
}