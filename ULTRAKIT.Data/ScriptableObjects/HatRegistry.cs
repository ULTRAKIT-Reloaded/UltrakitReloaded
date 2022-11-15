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
        public Transform transform;
        public EnemyType enemyType;
    }
}
