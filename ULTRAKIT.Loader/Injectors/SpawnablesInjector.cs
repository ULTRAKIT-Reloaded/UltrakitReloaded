using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using ULTRAKIT.Extensions;
using UnityEditor;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions.Data;
using ULTRAKIT.Extensions.Classes;

namespace ULTRAKIT.Loader.Injectors
{
    public static class SpawnablesInjector
    {
        public static List<SpawnableObject> _enemies = new List<SpawnableObject>();
        public static bool _init = false;

        private static AssetBundle Act2;
        private static List<string> Act2Scenes = new List<string>();

        // ULTRAKILL enemies added to the spawner arm by default
        static Dictionary<string, EnemyType> SpawnList = new Dictionary<string, EnemyType>
        {
            { "DroneFlesh", EnemyType.Drone },
            { "DroneSkull Variant", EnemyType.Drone },
            { "MinosBoss", EnemyType.Minos },
            { "Wicked", EnemyType.Wicked },
        };

        public static void Init()
        {
            // Loading an entire bundle and scene in the background can slow things down
            if (ConfigData.Leviathan)
                PrepLeviathan();
            else
                _init = true;

            foreach (var pair in SpawnList)
            {
                SpawnableObject spawnable = ScriptableObject.CreateInstance<SpawnableObject>();
                spawnable.identifier = pair.Key;
                spawnable.spawnableObjectType = SpawnableObject.SpawnableObjectDataType.Enemy;
                spawnable.objectName = pair.Key;
                spawnable.type = "Enemy";
                spawnable.enemyType = pair.Value;
                spawnable.spawnableType = SpawnableType.SimpleSpawn;
                spawnable.gameObject = GrabEnemy(pair.Key);
                spawnable.preview = new GameObject();
                spawnable.gridIcon = Registries.spawn_sprites[pair.Key];

                _enemies.Add(spawnable);
            }
        }

        private static void PrepLeviathan()
        {
            // Loads act 2 bundle if it isn't already loaded
            var data = File.ReadAllBytes($@"{Application.productName}_Data\StreamingAssets\acts\act-2");
            if (!AssetLoader.LoadFromLoaded(@"acts/act-2", out Act2))
                Act2 = AssetBundle.LoadFromMemory(data);

            // Collects a list of all act 2 scenes, then loads Level 5-4 additively
            string[] scenePaths = Act2.GetAllScenePaths();
            foreach (string scenePath in scenePaths)
                Act2Scenes.Add(Path.GetFileNameWithoutExtension(scenePath));
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[10]);

            // Sets the scene to finish the process when it loads
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public static GameObject GrabEnemy(string enemy)
        {
            GameObject obj = AssetLoader.AssetFind<GameObject>("common", enemy);
            if (obj != null)
            {
                SetHealthBar(obj, enemy);
                return obj;
            }
            
            obj = BossFind(enemy);

            if (obj == null)
            {
                UKLogger.LogWarning($"Could not find enemy {enemy}");
                return null;
            }

            SetHealthBar(obj, enemy);
            return obj;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // If the scene loaded isn't part of act 2, unload the bundle (but keep assets pulled from it, such as the leviathan) and remove this function
            // Since 5-4 is loaded immediately after subscribing to the event, any other scenes will be loaded after
            if (!Act2Scenes.Contains(scene.name))
            {
                if (Act2 != null)
                    Act2.Unload(false);
                SceneManager.sceneLoaded -= OnSceneLoaded;
                return;
            }
            if (scene.name == "Level 5-4")
            {
                // Avoids calling it again in case 5-4 is entered normally and the function remains subscribed to sceneLoaded
                if (_init) return;
                // Grabs a disabled copy of the leviathan that persists across scenes
                GameObject[] roots = scene.GetRootGameObjects();
                GameObject temp_levi = roots.Where(g => g.name == "Surface").First().transform.Find("Stuff/Boss/Leviathan").gameObject;
                GameObject leviathan = GameObject.Instantiate(temp_levi);
                GameObject.DontDestroyOnLoad(leviathan);
                leviathan.SetActive(false);

                SpawnableObject spawnable = ScriptableObject.CreateInstance<SpawnableObject>();
                    spawnable.identifier = "leviathan";
                    spawnable.spawnableObjectType = SpawnableObject.SpawnableObjectDataType.Enemy;
                    spawnable.objectName = "leviathan";
                    spawnable.type = "Enemy";
                    spawnable.enemyType = EnemyType.Leviathan;
                    spawnable.spawnableType = SpawnableType.SimpleSpawn;
                    spawnable.gameObject = leviathan;
                    spawnable.preview = new GameObject();
                    spawnable.gridIcon = Registries.spawn_sprites["Leviathan"];

                SetHealthBar(leviathan, "Leviathan");
                _enemies.Add(spawnable);
                SceneManager.UnloadSceneAsync("Level 5-4");
                _init = true;
                return;
            }
        }

        public static GameObject BossFind(string name)
        {
            //Find set Object in the prefabs
            GameObject[] Pool = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in Pool)
            {
                if (obj.gameObject.name == name && (obj.gameObject.tag == "Enemy" || name == "Wicked"))
                {
                    if (!obj.activeSelf) obj.SetActive(true);

                    // Fix lighting
                    var smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                    foreach (var item in smrs)
                    {
                        item.gameObject.layer = LayerMask.NameToLayer("Outdoors");
                    }
                    return obj;
                }
            }
            UKLogger.LogWarning($"Could not find boss {name}");
            return default;
        }

        public static void SetHealthBar(GameObject obj, string enemy)
        {
            BossHealthBar bhb = obj.GetComponentInChildren<BossHealthBar>();
            if (bhb == null && (enemy == "MinosBoss" || enemy == "Leviathan"))
            {
                bhb = obj.GetComponentInChildren<EnemyIdentifier>(true).gameObject.AddComponent<BossHealthBar>();
                bhb.bossName = "";
            }

            // Sets health bars to remain over the boss's head to avoid covering the entire screen
            CustomHealthbarPos cust = bhb?.gameObject.AddComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.offset = Vector3.up * 6;
                cust.enabled = false;
            }
        }
    }
}
