using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public class HatsManager : MonoBehaviour
    {
        public Dictionary<string, GameObject> hats;

        private SeasonalHats sh;

        private void Awake()
        {
            if (sh == null) sh = GetComponent<SeasonalHats>();
            hats = new Dictionary<string, GameObject>();
            hats.Add("christmas", sh.GetPrivate<GameObject>("christmas"));
            hats.Add("halloween", sh.GetPrivate<GameObject>("halloween"));
            hats.Add("easter", sh.GetPrivate<GameObject>("easter"));
        }

        public void LoadHat(HatRegistry registry)
        {
            Hat hat = ScriptableObject.CreateInstance<Hat>();
            try
            {
                Transform enemy = transform;
                while (enemy.GetComponent<EnemyIdentifier>() == null)
                {
                    enemy = enemy.parent;
                }
                hat = registry.hatDict[enemy.GetComponentInChildren<EnemyIdentifier>().enemyType];
            }
            catch
            {
                return;
            }
            GameObject hatInstance = Instantiate(hat.obj, gameObject.transform);
            hatInstance.transform.localPosition = hat.transform.position;
            hatInstance.transform.localRotation = hat.transform.rotation;
            PeterExtensions.RenderObject(hatInstance, LayerMask.NameToLayer("Limb"));
            hatInstance.SetActive(false);
            hats.Add(registry.hatID, hatInstance);
        }

        public void SetHatActive(string hatID, bool active)
        {
            if (!hats.ContainsKey(hatID))
            {
                return;
            }
            hats[hatID].SetActive(active);
        }
    }
}
