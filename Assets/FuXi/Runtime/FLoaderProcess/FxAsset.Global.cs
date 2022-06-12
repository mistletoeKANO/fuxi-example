using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuXi
{
    public partial class FxAsset
    {
        private static readonly Dictionary<string, FxAsset> AssetCache = new Dictionary<string, FxAsset>();

        internal static Func<string, Type, bool, FxAsset> FxAssetCreate;
        
        internal static FxAsset CreateAsset(string path, Type type, bool immediate)
        { return new FxAsset(path, type, immediate); }

        public static FxAsset LoadAsset<T>(string path)
        {
            var res = ReferenceAsset(path, typeof(T), true).Execute();
            return (FxAsset) res.Result;
        }

        public static async Task<FxAsset> LoadAssetAsync<T>(string path)
        {
            var res = await ReferenceAsset(path, typeof(T), false).Execute();
            return (FxAsset) res;
        }

        public static FxAsset LoadAsset(string path, Type type)
        {
            var res = ReferenceAsset(path, type, true);
            res.Execute();
            return res;
        }

        public static FxAsset LoadAssetCo(string path, Type type)
        {
            var res = ReferenceAsset(path, type, false);
            res.Execute();
            return res;
        }
        
        private static FxAsset ReferenceAsset(string path, Type type, bool immediate)
        {
            if (!AssetCache.TryGetValue(path, out var fxAsset))
            {
                fxAsset = FxAssetCreate.Invoke(path, type, immediate);
                AssetCache.Add(path, fxAsset);
            }
            fxAsset.AddReference();
            return fxAsset;
        }
    }
}