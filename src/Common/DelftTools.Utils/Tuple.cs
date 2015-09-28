using System;

namespace DelftTools.Utils
{
    /// <summary>
    /// This class was introduced to support the most simple as possible support for Tuples.
    /// the class DelftTools.DataObjects.Functions.Tuples.Pair has some restrictions.
    /// see http://luke.breuer.com/time/item/C_Tuple%5BT1,_T2%5D/219.aspx
    /// TODO in c# use dynamic type see: http://spellcoder.com/blogs/dodyg/archive/2008/10/30/16319.aspx
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Tuple<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Tuple()
        {
            
        }

        public Tuple(T1 first, T2 second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            First = first;
            Second = second;
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return object.ReferenceEquals(a, b) ||
                   (object)a != null && a.Equals(b);
        }

        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<T1, T2>;

            return
                t != null &&
                t.First.Equals(this.First) &&
                t.Second.Equals(this.Second);
        }

        public override string ToString()
        {
            return string.Format("First: {0}  Second: {1}", First, Second);
        }
    }
}