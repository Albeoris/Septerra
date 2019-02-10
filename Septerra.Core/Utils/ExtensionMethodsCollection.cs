using System;
using System.Collections.Generic;

namespace Septerra.Core
{
    public static class ExtensionMethodsCollection
    {
        public static T2[] SelectArray<T1,T2>(this IReadOnlyCollection<T1> collection, Func<T1, T2> selector)
        {
            T2[] result = new T2[collection.Count];

            Int32 index = 0;
            foreach (var item in collection)
                result[index++] = selector(item);

            return result;
        }
    }
}