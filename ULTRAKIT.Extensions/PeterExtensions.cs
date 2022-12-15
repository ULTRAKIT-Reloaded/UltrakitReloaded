using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Data;

namespace ULTRAKIT.Extensions
{
    public static class PeterExtensions
    {
        public static int FindIndexOf<T>(this T[] source, T obj)
        {
            int returnVal = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(obj))
                {
                    returnVal = i;
                    break;
                }
            }
            return returnVal;
        }

        public static int[] FindIdexesOf<T>(this T[] source, T obj)
        {
            List<int> returnVal = new List<int>();
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(obj))
                {
                    returnVal.Add(i);
                }
            }
            return returnVal?.ToArray() ?? new int[0];
        }

        public static Type GetInternalType(string _input)
        {
            Type type = typeof(GunControl).Assembly.GetType(_input);
            return type;
        }

        public static void RenderObject(GameObject obj, LayerMask layer)
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
                    c.material.shader = Shader.Find(c.material.shader.name);
                }
            }
        }

        public static void RemoveRange<T>(this List<T> list, T[] range)
        {
            foreach (T item in range)
                list.Remove(item);
        }

        public static void RemoveRange<T>(this List<T> list, List<T> range)
        {
            foreach (T item in range)
                list.Remove(item);
        }

        public static void ReplaceOrAddTo<T>(this List<T> list, T item, int index) where T : new()
        {
            while (index >= list.Count)
            {
                list.Add(new T());
            }
            list[index] = item;
        }
    }

    public class RenderFixer : MonoBehaviour
    {
        public void Start()
        {
            PeterExtensions.RenderObject(gameObject, LayerMask.NameToLayer("AlwaysOnTop"));
        }
    }
}
