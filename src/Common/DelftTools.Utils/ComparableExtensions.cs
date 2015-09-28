using System;

namespace DelftTools.Utils
{
    public static class ComparableExtensions
    {
        public static bool IsBigger(this IComparable object1, IComparable object2)
        {
            if (object1 == null)
            {
                return false; // null is not bigger than anything
            }
            
            return object1.CompareTo(object2) > 0;
        }

        public static bool IsSmaller(this IComparable object1, IComparable object2)
        {
            if (object1 == null)
            {
                return object2 != null; // smaller than anything but null
            }

            return object1.CompareTo(object2) < 0;
        }

        public static bool IsInRange(this IComparable value, IComparable limitOne, IComparable limitTwo)
        {
            IComparable min;
            IComparable max;

            if (limitOne.IsSmaller(limitTwo))
            {
                min = limitOne;
                max = limitTwo;
            }
            else
            {
                min = limitTwo;
                max = limitOne;
            }

            return (min.IsSmaller(value) && max.IsBigger(value)) || min.CompareTo(value) == 0 || max.CompareTo(value) == 0;
        }
    }
}