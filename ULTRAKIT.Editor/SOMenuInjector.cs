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
        [MenuItem("Assets/ULTRAKIT/Exporter")]
        public static void NewAssetBundleData()
        {
            CreateObject<AssetBundleData>("Exporter");
        }

        [MenuItem("Assets/ULTRAKIT/New Weapon")]
        public static void NewWeapon()
        {
            CreateObject<Weapon>("New_Weapon");
        }

        [MenuItem("Assets/ULTRAKIT/New Replacement Weapon")]
        public static void NewReplacementWeapon()
        {
            CreateObject<ReplacementWeapon>("New_Replacement_Weapon");
        }

        [MenuItem("Assets/ULTRAKIT/New Hat")]
        public static void NewHat()
        {
            CreateObject<Hat>("New_Hat");
        }

        [MenuItem("Assets/ULTRAKIT/New Hat Registry")]
        public static void NewHatRegistry()
        {
            CreateObject<HatRegistry>("New_Hat_Registry");
        }

        [MenuItem("Assets/ULTRAKIT/New Spawnable")]
        public static void NewSpawnable()
        {
            CreateObject<UKSpawnable>("New_Spawnable");
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
