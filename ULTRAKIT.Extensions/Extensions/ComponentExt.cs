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
        /// <summary>
        /// Returns all components of type T in an array of components.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns all components of type T in an array of components and their children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="searchInactive"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the transforms of all child objects, excluding the parent's transform.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<Transform> ListChildren(this Component parent)
        {
            List<Transform> children = new List<Transform>();
            children.AddRange(parent.GetComponentsInChildren<Transform>(true));
            children.Remove(parent.transform);
            return children;
        }

        /// <summary>
        /// Finds the first child with the given name of an object, searching recursively.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns>The transform of the child object, or `null` if none are found.</returns>
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
