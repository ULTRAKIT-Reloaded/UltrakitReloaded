using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class GraphicsUtilities
    {
        /// <summary>
        /// Creates a sprite of the specified size from an image. The file extension should be changed to `.bytes` prior to loading.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>A sprite</returns>
        public static Sprite CreateSprite(byte[] bytes, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            tex.LoadImage(bytes);
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(width/2, height/2));
        }

        /// <summary>
        /// Renders an object loaded from an asset bundle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        public static void RenderObject<T>(this T obj, LayerMask layer) where T : Component
        {
            foreach (var c in obj.GetComponentsInChildren<Renderer>(true))
            {
                c.gameObject.layer = layer;

                var glow = c.gameObject.GetComponent<Glow>();

                if (glow)
                {
                    c.material.shader = Shader.Find("psx/railgun");
                    c.material.SetFloat("_EmissivePosition", 5);
                    c.material.SetFloat("_EmissiveStrength", glow.glowIntensity);
                    c.material.SetColor("_EmissiveColor", glow.glowColor);
                }
                else
                {
                    // Why? I have no clue, but it works.
                    c.material.shader = Shader.Find(c.material.shader.name);
                }
            }
        }
    }
}
