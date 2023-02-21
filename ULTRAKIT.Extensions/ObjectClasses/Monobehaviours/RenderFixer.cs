using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ULTRAKIT.Extensions.ObjectClasses.Monobehaviours
{
    public class RenderFixer : MonoBehaviour
    {
        public string LayerName;

        public void Start()
        {
            this.RenderObject(LayerMask.NameToLayer(LayerName));
        }
    }
}
