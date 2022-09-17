using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace ULTRAKIT.EditorScripts
{
    public class SOMenuInjector
    {
        [MenuItem("Assets/ULTRAKIT/New_AssetBundle_Data")]
        public static void NewAssetBundleData()
        {
            CreateObject<AssetBundleData>("New_AssetBundle_Data");
        }

        [MenuItem("Assets/ULTRAKIT/New_Weapon")]
        public static void NewWeapon()
        {
            CreateObject<Weapon>("New_Weapon");
        }

        private static void CreateObject<T>(string name) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            string windowPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string finalPath = AssetDatabase.GenerateUniqueAssetPath($@"{windowPath}\{name}.asset");

            AssetDatabase.CreateAsset(obj, finalPath);
            AssetDatabase.Refresh();
            Selection.activeObject = obj;
        }
    }
}
