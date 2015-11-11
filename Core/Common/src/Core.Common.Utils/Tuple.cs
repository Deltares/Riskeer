using System;

namespace Core.Common.Utils
{
    /// <summary>
    /// This class was introduced to support the most simple as possible support for Tuples.
    /// see http://luke.breuer.com/time/item/C_Tuple%5BT1,_T2%5D/219.aspx
    /// TODO in c# use dynamic type see: http://spellcoder.com/blogs/dodyg/archive/2008/10/30/16319.aspx
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Tuple<T1, T2>
    {
        public Tuple(T1 first, T2 second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }

            First = first;
            Second = second;
        }

        public T1 First { get; set; }
        public T2 Second { get; set; }

        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return ReferenceEquals(a, b) ||
                   (object) a != null && a.Equals(b);
        }

        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<T1, T2>;

            return
                t != null &&
                t.First.Equals(First) &&
                t.Second.Equals(Second);
        }

        public override string ToString()
        {
            return string.Format("First: {0}  Second: {1}", First, Second);
        }
    }
}