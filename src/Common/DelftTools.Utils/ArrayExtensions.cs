using System;

namespace DelftTools.Utils
{
    public static class ArrayExtensions
    {
        public static int[] GetShape(this Array array)
        {
            int[] shape = new int[array.Rank];
            for (var i = 0; i < array.Rank; i++)
            {
                shape[i] = array.GetLength(i);
            }

            return shape;
        }
    }
}