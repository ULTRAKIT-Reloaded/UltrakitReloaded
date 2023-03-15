using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using ULTRAKIT.Extensions.Interfaces;
using ULTRAKIT.Extensions.ObjectClasses;
using UnityEngine;

namespace ULTRAKIT.Loader
{
    public static class Registries
    {
        // WeaponLoader
        public static Dictionary<string, List<Weapon>> weap_registry = new Dictionary<string, List<Weapon>>();
        public static List<Weapon> weap_allWeapons = new List<Weapon>();
        public static Dictionary<Tuple<WeaponType, int>, ReplacementWeapon> weap_replacements = new Dictionary<Tuple<WeaponType, int>, ReplacementWeapon>();

        // HatLoader
        public static List<HatRegistry> hat_registries = new List<HatRegistry>();
        public static List<string> hat_activeHats = new List<string>();

        // BuffLoader
        public static List<IBuff> buff_buffRegistry = new List<IBuff>();

        // SpawnablesLoader
        public static List<UKSpawnable> spawn_spawnables = new List<UKSpawnable>();
        public static SpawnableObjectsDatabase spawn_spawnablesDatabase = ScriptableObject.CreateInstance<SpawnableObjectsDatabase>();

        public static SpawnableObject[] spawn_tools = new SpawnableObject[0];
        public static SpawnableObject[] spawn_enemies = new SpawnableObject[0];
        public static SpawnableObject[] spawn_objects = new SpawnableObject[0];

        // SpawnablesInjector
        public static Dictionary<string, Sprite> spawn_sprites = new Dictionary<string, Sprite>();

        // OptionsLoader
        public static SortedList<string, UKSetting> options_registry = new SortedList<string, UKSetting>();
        public static Dictionary<string, GameObject> options_menus = new Dictionary<string, GameObject>();
        public static Dictionary<string, GameObject> options_buttons = new Dictionary<string, GameObject>();
        public static List<string> options_menusToAdd = new List<string>();

        // Keybinds
        public static Dictionary<string, UKKeySetting> key_registry = new Dictionary<string, UKKeySetting>();
        public static Dictionary<string, InputActionState> key_states = new Dictionary<string, InputActionState>();

        private static int counter = 0;
        public static void RegisterSetting(UKSetting setting)
        {
            options_registry.Add($"{setting.Section}{setting.Heading}{counter}", setting);
            counter++;
        }
    }
}
