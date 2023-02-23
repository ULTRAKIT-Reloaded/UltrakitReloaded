using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;

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
    }
}
