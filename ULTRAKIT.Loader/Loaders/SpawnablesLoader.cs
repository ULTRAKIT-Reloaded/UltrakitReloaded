using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Classes;
using UnityEngine;

namespace ULTRAKIT.Loader.Loaders
{
    public static class SpawnablesLoader
    {
        private static List<UKSpawnable> spawnables => Registries.spawn_spawnables;
        private static SpawnableObjectsDatabase spawnablesDatabase => Registries.spawn_spawnablesDatabase;

        public static bool init = false;

        /// <summary>
        /// Loads spawnables into the spawner arm automatically from a loaded asset bundle.
        /// </summary>
        /// <param name="bundle"></param>
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

        /// <summary>
        /// Removes spawnables from the registry.
        /// </summary>
        /// <param name="bundle"></param>
        public static void UnloadSpawnables(AssetBundle bundle)
        {
            UKSpawnable[] ukS = bundle.LoadAllAssets<UKSpawnable>();
            foreach (UKSpawnable ukSpawnable in ukS)
            {
                if (spawnables.Contains(ukSpawnable))
                    spawnables.Remove(ukSpawnable);
            }
        }

        /// <summary>
        /// Internal, do not use. Converts UKSpawnables into native SpawnableObjects.
        /// </summary>
        /// <param name="spawnMenu"></param>
        public static void InjectSpawnables(SpawnMenu spawnMenu)
        {
            List<SpawnableObject> tools = new List<SpawnableObject>();
            List<SpawnableObject> enemies = new List<SpawnableObject>();
            List<SpawnableObject> objects = new List<SpawnableObject>();

            foreach (UKSpawnable ukSpawnable in Registries.spawn_spawnables)
            {
                SpawnableObject spawnable = ScriptableObject.CreateInstance<SpawnableObject>();
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

            enemies.AddRange(Injectors.SpawnablesInjector._enemies);

            // Adds loaded spawnables onto the pre-existing list
            Registries.spawn_tools = spawnablesDatabase.sandboxTools.Concat(tools).ToArray();
            Registries.spawn_enemies = spawnablesDatabase.enemies.Concat(enemies).ToArray();
            Registries.spawn_objects = spawnablesDatabase.objects.Concat(objects).ToArray();
        }
    }
}
