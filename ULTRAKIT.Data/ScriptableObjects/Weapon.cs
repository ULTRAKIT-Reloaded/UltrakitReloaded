using UnityEngine;

namespace ULTRAKIT.Data
{
    public class Weapon : ScriptableObject
    {
        public string id;

        public string[] Names;

        [HideInInspector]
        public GameObject[] All_Variants;

        public GameObject[] Variants;
        public GameObject[] AltVariants;

        public Sprite[] Icons;

        public bool Unlocked = true;

        [HideInInspector]
        public int[] equipOrder;
        [HideInInspector]
        public int[] equipStatus;
        [HideInInspector]
        public string modName;
    }

    public class ReplacementWeapon : ScriptableObject
    {
        public GameObject Prefab;
        public WeaponType WeaponType;
        [Range(0, 2)]
        public int Variant;
        public bool Alt;

        [HideInInspector]
        public string modName;
    }

    public enum WeaponType
    {
        Revolver,
        Shotgun,
        Nailgun,
        Railcannon,
        RocketLauncher
    }
}
