using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Extensions
{
    public static class EnumerableExt
    {
        /// <summary>
        /// Finds the first index of an enumerable containing an equal value to the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <returns>The index of the object, or `0` if not found.</returns>
        public static int FindIndexOf<T>(this IEnumerable<T> source, T obj)
        {
            int returnVal = 0;
            for (int i = 0; i < source.Count(); i++)
            {
                if (source.ElementAt(i).Equals(obj))
                {
                    returnVal = i;
                    break;
                }
            }
            return returnVal;
        }

        /// <summary>
        /// Finds all indexes of an enumerable containing an equal value to the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <returns>An array of integers.</returns>
        public static int[] FindIdexesOf<T>(this IEnumerable<T> source, T obj)
        {
            List<int> returnVal = new List<int>();
            for (int i = 0; i < source.Count(); i++)
            {
                if (source.ElementAt(i).Equals(obj))
                {
                    returnVal.Add(i);
                }
            }
            return returnVal?.ToArray() ?? new int[0];
        }

        /// <summary>
        /// Removes a range of items from a list. The opposite of AddRange().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="range"></param>
        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
                list.Remove(item);
        }

        /// <summary>
        /// <para>Will replace the given index of a list if it exists, or will add default values up to that point to place an item at the required index.</para>
        /// <para>Useful if an exact index is required and others are ignored.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public static void ReplaceOrAddTo<T>(this List<T> list, T item, int index)
        {
            while (index >= list.Count)
            {
                list.Add(default);
            }
            list[index] = item;
        }
    }
}
