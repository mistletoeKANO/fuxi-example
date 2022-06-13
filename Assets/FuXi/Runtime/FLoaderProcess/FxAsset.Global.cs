using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuXi
{
    public partial class FxAsset
    {
        private static readonly Dictionary<string, FxAsset> AssetCache = new Dictionary<string, FxAsset>();

        internal static Func<string, Type, bool, Action<FxAsset>, FxAsset> FxAssetCreate;
        
        internal static FxAsset CreateAsset(string path, Type type, bool immediate, Action<FxAsset> callback)
        { return new FxAsset(path, type, immediate, callback); }

        public static FxAsset LoadAsset<T>(string path)
        {
            var res = ReferenceAsset(path, typeof(T), true, null).Execute();
            return (FxAsset) res.Result;
        }

        public static async Task<FxAsset> LoadAssetAsync<T>(string path)
        {
            var res = await ReferenceAsset(path, typeof(T), false, null).Execute();
            return (FxAsset) res;
        }

        public static FxAsset LoadAsset(string path, Type type)
        {
            var res = ReferenceAsset(path, type, true, null);
            res.Execute();
            return res;
        }

        public static FxAsset LoadAssetCo(string path, Type type)
        {
            var res = ReferenceAsset(path, type, false, null);
            res.Execute();
            return res;
        }

        public static void LoadAssetAsync(string path, Type type, Action<FxAsset> callback)
        {
            ReferenceAsset(path, type, false, callback).Execute();
        }
        
        private static FxAsset ReferenceAsset(string path, Type type, bool immediate, Action<FxAsset> callback)
        {
            if (!AssetCache.TryGetValue(path, out var fxAsset))
            {
                fxAsset = FxAssetCreate.Invoke(path, type, immediate, callback);
                AssetCache.Add(path, fxAsset);
            }
            fxAsset.AddReference();
            return fxAsset;
        }
    }
}