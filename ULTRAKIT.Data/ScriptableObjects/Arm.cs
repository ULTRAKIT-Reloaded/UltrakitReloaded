using UnityEngine;

namespace ULTRAKIT.Data
{
    public class Arm : ScriptableObject
    {
        public string id;

        public string Name;

        public GameObject Prefab;

        public Sprite Icon;

        public bool Unlocked = true;

        [HideInInspector]
        public string modName;
    }
}