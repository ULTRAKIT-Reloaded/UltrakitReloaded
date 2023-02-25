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
        /// <summary>
        /// Gets all objects of type T from an enumerable of generics.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allAssets"></param>
        /// <returns>An array of type T</returns>
        public static T[] GetAll<T>(IEnumerable<object> allAssets)
        {
            T[] assets = (T[])allAssets.Where(k => k is T).ToArray().Cast<T>();
            return assets;
        }

        /// <summary>
        /// <para>Searches through all loaded resources for an asset of type T with the given name.</para>
        /// <para>[NOTE] It is recommended to use the bundleName overload.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns>A Unity Object of type T</returns>
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

        /// <summary>
        /// Searches for an asset of type T with the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="name"></param>
        /// <returns>A Unity Object of type T</returns>
        public static T AssetFind<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
        {
            return bundle.LoadAsset<T>(name) ?? default;
        }

        /// <summary>
        /// Searches for an asset of type T with the given name in the specified bundle (often "common").
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundleName"></param>
        /// <param name="name"></param>
        /// <param name="searchActBundles"></param>
        /// <returns>A Unity Object of type T</returns>
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
                T asset = bundle.LoadAsset<T>(name) ?? default;
                bundle.Unload(false);
                return asset;
            }
            if (File.Exists(target2) && searchActBundles)
            {
                var data = File.ReadAllBytes(target2);
                bundle = AssetBundle.LoadFromMemory(data);
                T asset = bundle.LoadAsset<T>(name) ?? default;
                bundle.Unload(false);
                return asset;
            }

            UKLogger.LogWarning($"Could not find bundle {bundleName} or StreamingAssets file");
            return default;
        }

        /// <summary>
        /// Attempts to retrieve an already-loaded asset bundle, passing it through an `out` variable.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns>`true` if successful, otherwise false.</returns>
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
