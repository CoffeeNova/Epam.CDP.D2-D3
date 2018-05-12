using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class DictionaryExtensions
    {
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<TValue, bool> predicate)
        {
            var keys = dic.Keys.Where(x => predicate(dic[x])).ToList();
            foreach (var key in keys)
                dic.Remove(key);
        }
    }
}
