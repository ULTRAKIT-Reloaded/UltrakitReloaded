using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using UnityEngine;

namespace ULTRAKIT.Extensions
{
    public static class ComponentExt
    {
        public static IEnumerable<T> GetComponentsInArray<T>(this Component[] source) where T : Component
        {
            List<T> components = new List<T>();

            foreach (var obj in source)
            {
                T c;
                if (obj.TryGetComponent<T>(out c))
                    components.Add(c);
            }
            return components;
        }

        public static IEnumerable<T> GetAllComponentsInArray<T>(this Component[] source, bool searchInactive) where T : Component
        {
            List<T> components = new List<T>();

            foreach (var obj in source)
            {
                T[] c = obj.GetComponentsInChildren<T>(searchInactive);
                if (c.Length > 0)
                    components.AddRange(c);
            }

            return components;
        }

        public static IEnumerable<Transform> ListChildren(this Component parent)
        {
            List<Transform> children = new List<Transform>();
            children.AddRange(parent.GetComponentsInChildren<Transform>(true));
            children.Remove(parent.transform);
            return children;
        }

        public static Transform FindInChildren(this Component parent, string name)
        {
            foreach (Transform t in parent.GetComponentsInChildren<Transform>())
            {
                if (t.name == name) return t;
            }
            UKLogger.LogWarning($"Could not find child {name}");
            return default;
        }
    }
}
