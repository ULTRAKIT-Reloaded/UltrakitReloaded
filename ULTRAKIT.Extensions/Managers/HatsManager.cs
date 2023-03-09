using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;

namespace ULTRAKIT.Extensions.Managers
{
    public class HatsManager : MonoBehaviour
    {
        public static InputActionState state;
        // DELETE
        private void Update()
        {
            if (state.WasPerformedThisFrame)
                UKLogger.Log("PRESSED BUTTON");
        }

        public Dictionary<string, GameObject> hats;

        private SeasonalHats sh;

        private void Awake()
        {
            if (sh == null) sh = GetComponent<SeasonalHats>();
            hats = new Dictionary<string, GameObject>();
            hats.Add("christmas", sh.GetFieldValue<GameObject>("christmas", true));
            hats.Add("halloween", sh.GetFieldValue<GameObject>("halloween", true));
            hats.Add("easter", sh.GetFieldValue<GameObject>("easter", true));
        }

        /// <summary>
        /// Used internally. Loads a registered set of hats onto an enemy.
        /// </summary>
        /// <param name="registry"></param>
        public void LoadHat(HatRegistry registry)
        {
            Hat hat = ScriptableObject.CreateInstance<Hat>();
            try
            {
                // Starts wherever SeasonalHats is placed on an enemy, steps back until it find their root
                Transform enemy = transform;
                while (enemy.GetComponent<EnemyIdentifier>() == null && enemy.tag != "Player")
                {
                    enemy = enemy.parent;
                }
                if (enemy.tag == "Player") hat = registry.hatDict[EnemyType.V2];
                // Mandalore, Cancerous Rodent and Very Cancerous Rodent, despite having their own EnemyTypes, use Drone, Husk, and Cerberus, respectively, so I am forced to check their gameObject names.
                else
                {
                    EnemyType type = enemy.GetComponentInChildren<EnemyIdentifier>().enemyType;
                    hat = registry.hatDict[type];
                    if (type == EnemyType.Drone && enemy.name.Contains("Mandalore")) hat = registry.hatDict[EnemyType.Mandalore];
                }
                if (transform.parent.name.Contains("Cancerous Rodent")) hat = registry.hatDict[EnemyType.CancerousRodent];
                if (transform.parent.name.Contains("Very Cancerous Rodent")) hat = registry.hatDict[EnemyType.VeryCancerousRodent];
            }
            // Skips enemies with no matching hat
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

        /// <summary>
        /// Enables/disables the hat with the given ID. Ignores invalid IDs.
        /// </summary>
        /// <param name="hatID"></param>
        /// <param name="active"></param>
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
