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

        static Dictionary<string, EnemyType> SpawnList = new Dictionary<string, EnemyType>
        {
            { "DroneFlesh", EnemyType.Drone },
            { "DroneSkull Variant", EnemyType.Drone },
            { "MinosBoss", EnemyType.Minos },
            { "Wicked", EnemyType.Wicked },
        };

        public static void Init()
        {
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
                /*switch (pair.Key)
                {
                    case "DroneFlesh": spawnable.gridIcon = fpeye; break;
                    case "DroneSkull Variant": spawnable.gridIcon = fpface; break;
                    case "MinosBoss": spawnable.gridIcon = minos; break;
                    case "Wicked": spawnable.gridIcon = wicked; break;
                }*/

                _enemies.Add(spawnable);
            }
        }

        private static void PrepLeviathan()
        {
            var data = File.ReadAllBytes($@"{Application.productName}_Data\StreamingAssets\acts\act-2");
            if (!AssetLoader.LoadFromLoaded(@"acts/act-2", out Act2))
                Act2 = AssetBundle.LoadFromMemory(data);

            string[] scenePaths = Act2.GetAllScenePaths();
            foreach (string scenePath in scenePaths)
                Act2Scenes.Add(Path.GetFileNameWithoutExtension(scenePath));
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[10]);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

            SceneManager.sceneLoaded += OnSceneLoaded;
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
            if (!Act2Scenes.Contains(scene.name))
            {
                if (Act2 != null)
                    Act2.Unload(false);
                SceneManager.sceneLoaded -= OnSceneLoaded;
                return;
            }
            if (scene.name == "Level 5-4")
            {
                if (_init) return;
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
                    if (obj.activeSelf != true) obj.SetActive(true);

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

            CustomHealthbarPos cust = bhb?.gameObject.AddComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.offset = Vector3.up * 6;
                cust.enabled = false;
            }
        }
    }
}
