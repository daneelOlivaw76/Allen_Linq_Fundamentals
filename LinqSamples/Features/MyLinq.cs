using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace Features
{
    public static class MyLinq
    {
        /// <summary>
        /// A new Count method for IEnumerables.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static int Count<T>(this IEnumerable<T> sequence)
        {
            int count = 0;
            foreach(var item in sequence)
            {
                count += 1;
            }
            return count;
        }
    }
}
