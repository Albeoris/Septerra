using System.Collections.Generic;

namespace Septerra.Core
{
    public static class ExtensionMethodsDictionary
    {
        public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            Dictionary<TValue, TKey> result = new Dictionary<TValue, TKey>(self.Count);
            foreach (var pair in self)
                result.Add(pair.Value, pair.Key);
            return result;
        }
    }
}