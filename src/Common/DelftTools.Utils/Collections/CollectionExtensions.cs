using System;
using System.Collections.Generic;
using System.Linq;

namespace DelftTools.Utils.Collections
{
    public static class CollectionExtensions
    {
        public static void RemoveAllWhere<T>(this ICollection<T> source, Func<T, bool> condition)
        {
            var list = source.Where(condition).ToList();
            foreach (T item in list)
            {
                source.Remove(item);
            }
        }
        
    }
}