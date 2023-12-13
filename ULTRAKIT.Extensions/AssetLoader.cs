using GameConsole.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace ULTRAKIT.Extensions
{
    public static class AssetLoader
    {
        public static Dictionary<string, string> ShortToFullAssets = new Dictionary<string, string>();

        internal static void Init(string unparsedAssetData)
        {
            string[] assets = unparsedAssetData.Split(new string[] { "=================================" }, StringSplitOptions.None);
            assets = assets.Skip(1).Select(s => s.Split('\n')[1]).ToArray();
            assets = assets.Select(s => s.Substring(6)).ToArray();
            foreach (string asset in assets)
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
        /// <para>Loads an asset of type T with the given name.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns>A Unity Object of type T</returns>
        public static T AssetFind<T>(string name) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = LoadAsset(name);
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

        internal static UnityEngine.Object LoadAsset(string name)
        {
            if (ShortToFullAssets.ContainsKey(name))
                name = ShortToFullAssets[name];
            UnityEngine.Object obj = Addressables.LoadAssetAsync<UnityEngine.Object>(name).WaitForCompletion();
            if (obj == null)
                UKLogger.LogWarning($"Asset {name} not found");
            return obj;
        }
    }
}
