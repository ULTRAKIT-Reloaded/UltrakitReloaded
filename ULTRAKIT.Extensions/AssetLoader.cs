using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace ULTRAKIT.Extensions
{
    public static class AssetLoader
    {
        public static object LoadAsset(string filePath, string name)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            object asset = bundle.LoadAsset(name);

            Debug.Log($"Loaded asset {name} from {filePath}");
            return asset;
        }

        public static object[] LoadAllAssets(string filePath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            object[] assets = bundle.LoadAllAssets();
            return assets;
        }

        public static object[] LoadAssets(string filePath, string[] names)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            List<object> assets = new List<object>();

            foreach (string name in names)
            {
                assets.Add(bundle.LoadAsset(name));
            }

            return assets.ToArray();
        }

        public static AssetBundleRequest LoadAssetAsync(string filePath, string name)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            return bundle.LoadAssetAsync(name);
        }

        public static AssetBundleRequest LoadAllAssetsAsync(string filePath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            return bundle.LoadAllAssetsAsync();
        }

        public static List<AssetBundleRequest> LoadAssetsAsync(string filePath, string[] names)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            List<AssetBundleRequest> requests = new List<AssetBundleRequest>();

            foreach (string name in names)
            {
                requests.Add(bundle.LoadAssetAsync(name));
            }

            return requests;
        }

        public static T[] GetAll<T>(object[] allAssets)
        {
            T[] assets = (T[])allAssets.Where(k => k is T).ToArray().Cast<T>();
            return assets;
        }
    }
}
