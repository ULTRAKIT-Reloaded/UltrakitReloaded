using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Data
{
    public class UKSpawnable : ScriptableObject
    {
        public SpawnableObject.SpawnableObjectDataType type;
        public string identifier;
        public GameObject prefab;
        public Sprite icon;
    }
}
