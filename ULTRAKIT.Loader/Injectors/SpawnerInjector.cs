using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using ULTRAKIT.Extensions;
using static UnityEngine.EventSystems.EventTrigger;

namespace ULTRAKIT.Loader.Injectors
{
    public static class SpawnerInjector
    {
        public static Sprite fpeye;
        public static Sprite fpface;
        public static Sprite minos;
        public static Sprite wicked;
        public static Sprite levi;

        public static bool _init = false;
        static AssetBundle Common;
        static AssetBundle Act2;

        static Dictionary<string, EnemyType> SpawnList = new Dictionary<string, EnemyType> {
            { "DroneFlesh", EnemyType.Drone },
            { "DroneSkull Variant", EnemyType.Drone },
            { "MinosBoss", EnemyType.Minos },
            { "Wicked", EnemyType.Wicked },
        };

        static List<string> SceneBlackList = new List<string>();

        public static List<SpawnableObject> _enemies = new List<SpawnableObject>();

        public static void Init()
        {
            if (File.Exists($@"{Application.productName}_Data\StreamingAssets\acts\act-2"))
            {
                var data = File.ReadAllBytes($@"{Application.productName}_Data\StreamingAssets\acts\act-2");
                Act2 = DazeExtensions.LoadFromLoaded(Act2, @"acts/act-2") ?? AssetBundle.LoadFromMemory(data);
            }
            string[] scenePaths = Act2.GetAllScenePaths();
            foreach (string scenePath in scenePaths)
                SceneBlackList.Add(Path.GetFileNameWithoutExtension(scenePath));
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[10]);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

            SceneManager.sceneLoaded += OnSceneLoaded;

            foreach (var pair in SpawnList)
            {
                SpawnableObject spawnable = SpawnableObject.CreateInstance<SpawnableObject>();
                spawnable.identifier = pair.Key;
                spawnable.spawnableObjectType = SpawnableObject.SpawnableObjectDataType.Enemy;
                spawnable.objectName = pair.Key;
                spawnable.type = "Enemy";
                spawnable.enemyType = pair.Value;
                spawnable.spawnableType = SpawnableType.SimpleSpawn;
                GameObject enemy = GrabEnemy(pair.Key);
                Debug.Log($"Loading {pair.Key}");
                Debug.Log(enemy?.gameObject?.name ?? "null");
                spawnable.gameObject = enemy;
                spawnable.preview = new GameObject();
                switch (pair.Key)
                {
                    case "DroneFlesh": spawnable.gridIcon = fpeye; break;
                    case "DroneSkull Variant": spawnable.gridIcon = fpface; break;
                    case "MinosBoss": spawnable.gridIcon = minos; break;
                    case "Wicked": spawnable.gridIcon = wicked; break;
                }

                _enemies.Add(spawnable);
            }
        }

        public static GameObject GrabEnemy(string enemy)
        {
            GameObject obj = new GameObject();
            GameObject tempObj = Common.PrefabFind("common", enemy);
            if (tempObj != null)
                obj = tempObj;
            else
            {
                try { obj = BossFind(enemy); }
                catch { Debug.Log("Error"); }
            }
            Debug.Log(obj?.name ?? "null");

            var bhb = obj.GetComponentInChildren<BossHealthBar>();
            if (bhb == null && (enemy == "MinosBoss" || enemy == "Leviathan"))
            {
                bhb = obj.GetComponentInChildren<EnemyIdentifier>(true).gameObject.AddComponent<BossHealthBar>();
                bhb.bossName = "";
            }

            var cust = bhb?.gameObject.AddComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.offset = Vector3.up * 6;
                cust.enabled = false;
            }

            return obj;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!SceneBlackList.Contains(scene.name))
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
                GameObject obj = roots.Where(g => g.name == "Surface").First().transform.Find("Stuff/Boss/Leviathan").gameObject;
                GameObject leviathan = GameObject.Instantiate(obj);
                GameObject.DontDestroyOnLoad(leviathan);
                leviathan.SetActive(false);

                SpawnableObject spawnable = SpawnableObject.CreateInstance<SpawnableObject>();
                spawnable.identifier = "leviathan";
                spawnable.spawnableObjectType = SpawnableObject.SpawnableObjectDataType.Enemy;
                spawnable.objectName = "leviathan";
                spawnable.type = "Enemy";
                spawnable.enemyType = EnemyType.Leviathan;
                spawnable.spawnableType = SpawnableType.SimpleSpawn;
                spawnable.gameObject = leviathan;
                spawnable.preview = new GameObject();
                spawnable.gridIcon = levi;

                var bhb = leviathan.GetComponentInChildren<BossHealthBar>();
                if (bhb == null)
                {
                    bhb = obj.GetComponentInChildren<EnemyIdentifier>(true).gameObject.AddComponent<BossHealthBar>();
                    bhb.bossName = "";
                }

                var cust = bhb?.gameObject.AddComponent<CustomHealthbarPos>();
                if (cust)
                {
                    cust.offset = Vector3.up * 6;
                    cust.enabled = false;
                }

                _enemies.Add(spawnable);
                SceneManager.UnloadSceneAsync("Level 5-4");
                _init = true;
            }
        }

        public static GameObject BossFind(string name)
        {
            //Find set Object in the prefabs
            GameObject[] Pool = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject a = null;
            foreach (GameObject obj in Pool)
            {
                if (obj.gameObject.name == name)
                {
                    if (obj.gameObject.tag == "Enemy" || name == "Wicked")
                    {
                        if (obj.activeSelf != true) obj.SetActive(true);
                        a = obj;

                        // Fix lighting
                        var smrs = a.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                        foreach (var item in smrs)
                        {
                            item.gameObject.layer = LayerMask.NameToLayer("Outdoors");
                        }
                    }
                }
            }
            return a;
        }
    }
}
