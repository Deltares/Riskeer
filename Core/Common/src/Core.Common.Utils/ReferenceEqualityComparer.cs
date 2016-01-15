using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Common.Utils
{
    /// <summary>
    /// This class determines whether two objects are equal based on their references.
    /// </summary>
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}