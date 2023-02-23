using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULTRAKIT.Extensions
{
    public static class EnumerableExt
    {
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

        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
                list.Remove(item);
        }

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
