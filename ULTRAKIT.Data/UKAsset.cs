using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ULTRAKIT.Data
{
    public class UKAsset : AssetReference
    {
        private UnityEngine.Object _asset;

        public override UnityEngine.Object Asset => _asset;

        public UKAsset(UnityEngine.Object asset)
        {
            _asset = asset;
        }
    }
}
