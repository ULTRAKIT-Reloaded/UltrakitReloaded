using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEditor;
using UnityEngine;

namespace ULTRAKIT.EditorScripts
{
    [CustomEditor(typeof(AssetBundleData))]
    public class AssetBundleEditor : Editor
    {
        static string outputPath = "";

        public override void OnInspectorGUI()
        {
            var data = target as AssetBundleData;
            var assetPath = AssetDatabase.GetAssetPath(data.GetInstanceID());

            EditorGUILayout.LabelField("EXPORT", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            outputPath = EditorGUILayout.TextField(outputPath);
            if (GUILayout.Button("Browse"))
            {
                outputPath = EditorUtility.SaveFilePanel("Export AssetBundle", "Assets", "modAssets", "assetBundle");
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Export AssetBundle"))
            {
                if (outputPath == null || outputPath.Length == 0)
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Please choose a valid output directory", "OK");
                    return;
                }

                var assetLabel = AssetDatabase.GetImplicitAssetBundleName(assetPath);
                var buildFailed = !BuildAssetBundleByName(assetLabel, outputPath);
                if (buildFailed) Debug.LogError("AssetBundle export failed");
            }
        }

        public static bool BuildAssetBundleByName(string name, string outputPath)
        {
            if (File.Exists(outputPath))
            {
                if (!EditorUtility.DisplayDialog("Replace File", $@"File '{Path.GetFileName(outputPath)}' already exists. Would you like to replace it?", "Yes", "No"))
                {
                    return false;
                }
            }

            var tempPath = @"Assets/_tempexport";
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);

            var build = new AssetBundleBuild();
            build.assetBundleName = name;
            build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(name);

            BuildPipeline.BuildAssetBundles(tempPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            try
            {
                File.Copy($@"{tempPath}/{name}", outputPath, true);
            }
            catch (ArgumentException)
            {
                Debug.LogError("AssetBundle has no name. Did you forget to assign one to the folder in the editor?");
                return false;
            }

            Directory.Delete(tempPath, true);

            if (outputPath.Contains("Assets/"))
            {
                AssetDatabase.Refresh();
            }
            Debug.Log($@"AssetBundle export successful at {outputPath}");
            return true;
        }
    }
}
