using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions.Interfaces;

namespace ULTRAKIT.Extensions.Managers
{
    public class BuffsManager : MonoBehaviour
    {
        public Dictionary<string, IBuff> buffs = new Dictionary<string, IBuff>();
        public EnemyIdentifier eid;

        /// <summary>
        /// Used internally. Loads registered buffs onto an enemy.
        /// </summary>
        /// <param name="buffsToLoad"></param>
        public void LoadBuffs(IBuff[] buffsToLoad)
        {
            foreach (IBuff buff in buffsToLoad)
            {
                buff.eid = eid;
                buffs.Add(buff.id, buff);
            }
        }

        /// <summary>
        /// Enables/disables the buff with the given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="active"></param>
        public void SetBuffState(string id, bool active)
        {
            if (!buffs.ContainsKey(id)) return;
            if (active && !buffs[id].IsActive)
            {
                buffs[id].Enable();
                return;
            }
            if (!active && buffs[id].IsActive)
            {
                buffs[id].Disable();
                return;
            }
        }

        private void Update()
        {
            foreach (var pair in buffs)
            {
                if (pair.Value.IsActive) pair.Value.Update();
            }
        }
    }
}
