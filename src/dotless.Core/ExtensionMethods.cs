using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotless.Core
{
    public static class ExtensionMethods
    {
        public static List<V> SelectList<T, V>(this IList<T> list, Func<T, V> del)
        {
            var ret = new List<V>(list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                ret.Add(del(list[i]));
            }

            return ret;
        }

        public static V[] SelectArray<T, V>(this IList<T> list, Func<T, V> del)
        {
            var ret = new V[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                ret[i] = del(list[i]);
            }

            return ret;
        }

        public static V[] SelectArray<T, V>(this IList<T> list, Func<T, int, V> del)
        {
            var ret = new V[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                ret[i] = del(list[i], i);
            }

            return ret;
        }

        public static V[] CastArray<T, V>(this IList<T> list)
        {
            var v = new V[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                v[i] = (V)(object)list[i];
            }

            return v;
        }
    }
}
