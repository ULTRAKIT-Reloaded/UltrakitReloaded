using UnityEngine;

namespace ULTRAKIT.Data
{
    public class Weapon : ScriptableObject
    {
        public string[] Names;
        public string[] AltNames;

        public GameObject[] Variants;
        public GameObject[] AltVariants;

        public Sprite[] Icons;
        public Sprite[] AltIcons;
    }
}
