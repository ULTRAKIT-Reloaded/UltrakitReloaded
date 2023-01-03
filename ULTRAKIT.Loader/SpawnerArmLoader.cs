using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using UnityEngine;

namespace ULTRAKIT.Loader
{
    public static class SpawnerArmLoader
    {
        public static List<UKSpawnable> spawnables = new List<UKSpawnable>();
        public static SpawnableObjectsDatabase spawnablesDatabase = ScriptableObject.CreateInstance<SpawnableObjectsDatabase>();

        public static SpawnableObject[] _tools = new SpawnableObject[0];
        public static SpawnableObject[] _enemies = new SpawnableObject[0];
        public static SpawnableObject[] _objects = new SpawnableObject[0];

        public static bool init = false;

        public static void LoadSpawnables(AssetBundle bundle)
        {
            UKSpawnable[] ukS = bundle.LoadAllAssets<UKSpawnable>();
            foreach (UKSpawnable ukSpawnable in ukS)
            {
                ukSpawnable.prefab.AddComponent<RenderFixer>().LayerName = "Outdoors";
                if (!spawnables.Contains(ukSpawnable))
                    spawnables.Add(ukSpawnable);
            }
        }

        public static void UnLoadSpawnables(AssetBundle bundle)
        {
            UKSpawnable[] ukS = bundle.LoadAllAssets<UKSpawnable>();
            foreach (UKSpawnable ukSpawnable in ukS)
            {
                if (spawnables.Contains(ukSpawnable))
                    spawnables.Remove(ukSpawnable);
            }
        }

        public static void InjectSpawnables(SpawnMenu spawnMenu)
        {
            List<SpawnableObject> tools = new List<SpawnableObject>();
            List<SpawnableObject> enemies = new List<SpawnableObject>();
            List<SpawnableObject> objects = new List<SpawnableObject>();

            foreach (UKSpawnable ukSpawnable in spawnables)
            {
                SpawnableObject spawnable = SpawnableObject.CreateInstance<SpawnableObject>();
                spawnable.identifier = ukSpawnable.identifier;
                spawnable.spawnableObjectType = ukSpawnable.type;
                spawnable.objectName = ukSpawnable.identifier;
                spawnable.type = "UKSpawnable";
                spawnable.enemyType = EnemyType.MinosPrime;
                spawnable.gameObject = ukSpawnable.prefab;
                spawnable.preview = new GameObject();
                spawnable.gridIcon = ukSpawnable.icon;
                switch (ukSpawnable.type)
                {
                    case SpawnableObject.SpawnableObjectDataType.Tool: 
                        spawnable.spawnableType = SpawnableType.ToolHand; 
                        tools.Add(spawnable);
                        break;
                    case SpawnableObject.SpawnableObjectDataType.Enemy: 
                        spawnable.spawnableType = SpawnableType.SimpleSpawn; 
                        enemies.Add(spawnable);
                        break;
                    case SpawnableObject.SpawnableObjectDataType.Object: 
                        spawnable.spawnableType = SpawnableType.SimpleSpawn; 
                        objects.Add(spawnable);
                        break;
                }
            }

            enemies.AddRange(Injectors.SpawnerInjector._enemies);

            _tools = spawnablesDatabase.sandboxTools.Concat(tools).ToArray();
            _enemies = spawnablesDatabase.enemies.Concat(enemies).ToArray();
            _objects = spawnablesDatabase.objects.Concat(objects).ToArray();
        }
    }
}
