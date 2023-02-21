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
                while (enemy.GetComponent<EnemyIdentifier>() == null && enemy.tag != "Player")
                {
                    enemy = enemy.parent;
                }
                if (enemy.tag == "Player") hat = registry.hatDict[EnemyType.V2];
                else 
                {
                    EnemyType type = enemy.GetComponentInChildren<EnemyIdentifier>().enemyType;
                    hat = registry.hatDict[type];
                    if (type == EnemyType.Drone && enemy.name.Contains("Mandalore")) hat = registry.hatDict[EnemyType.Mandalore];
                }
                if (transform.parent.name.Contains("Cancerous Rodent")) hat = registry.hatDict[EnemyType.CancerousRodent];
                if (transform.parent.name.Contains("Very Cancerous Rodent")) hat = registry.hatDict[EnemyType.VeryCancerousRodent];
            }
            catch
            {
                return;
            }
            GameObject hatInstance = Instantiate(hat.obj, gameObject.transform);
            hatInstance.transform.localPosition = hat.obj.transform.position + hat.position_offset;
            hatInstance.transform.localRotation = hat.obj.transform.rotation * Quaternion.Euler(hat.rotation_offset);
            hatInstance.transform.localScale = hat.obj.transform.localScale + hat.scale_offset;
            hatInstance.transform.RenderObject(LayerMask.NameToLayer("Limb"));
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
