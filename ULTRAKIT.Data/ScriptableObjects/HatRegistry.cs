using System.Collections.Generic;
using UnityEngine;

namespace ULTRAKIT.Data
{
    public class HatRegistry : ScriptableObject
    {
        public string hatID;
        public Hat[] hats;

        [HideInInspector]
        public Dictionary<EnemyType, Hat> hatDict;
    }

    public class Hat : ScriptableObject
    {
        public GameObject obj;
        public Vector3 position_offset;
        public Vector3 rotation_offset;
        public Vector3 scale_offset;
        public EnemyType enemyType;
    }
}
