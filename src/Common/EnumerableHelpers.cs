using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class EnumerableHelpers
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static IEnumerable<IEnumerable<T>> DivideByLength<T>(this IEnumerable<T> initialList, int chunkLength)
        {
            var enumerable = initialList as IList<T> ?? initialList.ToList();
            var valuesLength = enumerable.Count;
            var chunks = (int)Math.Ceiling(valuesLength / (double)chunkLength);
            var dividedList = Enumerable.Range(0, chunks).Select(i => enumerable.Skip(i * chunkLength).Take(chunkLength));
            return dividedList;
        }
    }
}
