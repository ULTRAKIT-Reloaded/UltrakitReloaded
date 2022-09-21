using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            return returnVal.ToArray();
        }

        public static Type GetInternalType(string _input)
        {
            Type type = typeof(GunControl).Assembly.GetType(_input);
            return type;
        }
    }
}
