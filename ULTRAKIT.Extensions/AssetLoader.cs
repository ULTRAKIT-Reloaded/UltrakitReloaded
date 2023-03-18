using GameConsole.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class AssetLoader
    {
        public static Dictionary<string, string> ShortToFullAssets = new Dictionary<string, string>();

        internal static void Init()
        {
            foreach (string asset in AssetManager.instance.assetDependencies.Keys)
            {
                string trim = asset.Split('/').Last();
                if (!ShortToFullAssets.ContainsKey(trim))
                    ShortToFullAssets.Add(trim, asset);
            }
        }

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
            string bundle = AssetManager.Instance.assetDependencies[name];
            AssetManager.Instance.LoadBundles(new string[1] { bundle });
            UnityEngine.Object obj = LoadAsset(bundle, name);
            if (obj == null)
            {
                UKLogger.LogError($"Failed to load asset {name}");
                return null;
            }
            if (!(obj is T result))
            {
                UKLogger.LogError($"Asset {name} is not a(n) {typeof(T)}");
                return null;
            }
            return result;
        }

        internal static UnityEngine.Object LoadAsset(string bundle, string name)
        {
            if (!ShortToFullAssets.ContainsKey(name))
            {
                UKLogger.LogWarning($"Asset {name} not found");
                return null;
            }
            UnityEngine.Object obj = AssetManager.Instance.loadedBundles[bundle].LoadAsset(ShortToFullAssets[name]);
            return obj;
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
