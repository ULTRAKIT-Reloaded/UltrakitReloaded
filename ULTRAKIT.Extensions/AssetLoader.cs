using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class AssetLoader
    {
        public static T[] GetAll<T>(object[] allAssets)
        {
            T[] assets = (T[])allAssets.Where(k => k is T).ToArray().Cast<T>();
            return assets;
        }

        public static T AssetFind<T>(string name) where T : UnityEngine.Object
        {
            T result = default;
            T[] allAssets = Resources.FindObjectsOfTypeAll<T>();

            foreach (T asset in allAssets)
                if (asset.name == name) result = asset;
            if (result == default)
                UKLogger.LogWarning($"Could not find asset {name}");
            return result;
        }

        public static T AssetFind<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
        {
            return bundle.LoadAsset<T>(name) ?? default;
        }

        public static T AssetFind<T>(string bundleName, string name, bool searchActBundles = false) where T : UnityEngine.Object
        {
            AssetBundle bundle;
            if (LoadFromLoaded(bundleName, out bundle))
            {
                return bundle.LoadAsset<T>(name) ?? default;
            }

            string target = $@"{Application.productName}_Data\StreamingAssets\{bundleName}";
            string target2 = $@"{Application.productName}_Data\StreamingAssets\acts\{bundleName}";
            if (File.Exists(target))
            {
                var data = File.ReadAllBytes(target);
                bundle = AssetBundle.LoadFromMemory(data);
                return bundle.LoadAsset<T>(name) ?? default;
            }
            if (File.Exists(target2) && searchActBundles)
            {
                var data = File.ReadAllBytes(target2);
                bundle = AssetBundle.LoadFromMemory(data);
                return bundle.LoadAsset<T>(name) ?? default;
            }

            UKLogger.LogWarning($"Could not find bundle {bundleName} or StreamingAssets file");
            return default;
        }

        public static bool LoadFromLoaded(string name, out AssetBundle result)
        {
            result = null;
            IEnumerable<AssetBundle> loadedBundles = AssetBundle.GetAllLoadedAssetBundles();

            foreach (AssetBundle bundle in loadedBundles)
                if (bundle.name == name)
                {
                    result = bundle;
                    return true;
                }

            return false;
        }
    }
}
