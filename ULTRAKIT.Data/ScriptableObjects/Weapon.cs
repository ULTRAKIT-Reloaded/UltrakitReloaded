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

        public bool EnabledByDefault = true;

        [HideInInspector]
        public int[] equipOrder;
        [HideInInspector]
        public int[] equipStatus;
        [HideInInspector]
        public string modName;
    }
}
