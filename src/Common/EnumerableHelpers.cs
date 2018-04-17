using System.Collections.Generic;

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
    }
}
