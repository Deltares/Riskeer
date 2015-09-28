using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DelftTools.Utils.NetCdf
{
    public static class NetCdfFileHelper
    {
        public static int GetSize(int[] shape)
        {
            return shape.Aggregate(1, (current, t) => current*t);
        }

        public static IntPtr[] ConvertToIntPtr(int[] array)
        {
            var result = new IntPtr[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = new IntPtr(array[i]);
            }
            return result;
        }

        public static Array CreateArrayFromShape<T>(T[] byteArray, int[] shape)
        {
            if (byteArray.Rank == shape.Length) // some assumptions here..
                return byteArray;

            var array = Array.CreateInstance(typeof (T), shape);
            Buffer.BlockCopy(byteArray, 0, array, 0, array.Length*Marshal.SizeOf(typeof(T)));
            return array;
        }

        /// <summary>
        /// returns jagged array (char[][]) so shape is assumed to have length 2!
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static char[][] CreateCharArrayFromShape(byte[] byteArray, int[] shape)
        {
            var nrOfCharArrays = shape[0];
            var charArraySize = shape[1];

            var charArray = new char[nrOfCharArrays][];
            int index = 0;
            for (int i = 0; i < nrOfCharArrays; ++i)
            {
                charArray[i] = new char[charArraySize];
                for (int j = 0; j < charArraySize; ++j)
                {
                    charArray[i][j] = (char)byteArray[index++];
                }
            }

            return charArray;
        }

        public static T[] FlattenArray<T>(Array array)
        {
            var target = new T[array.Length];
            Buffer.BlockCopy(array, 0, target, 0, target.Length * Marshal.SizeOf(typeof(T)));
            return target;
        }

        public static byte[] FlattenCharArray(Array array, int[] shape)
        {
            var type = array.GetType().GetElementType();
            
            if (array.Rank == 1 && type == typeof(char[]))
            {
                int index = 0;
                var bytes = new byte[shape[0] * shape[1]];
                
                // cannot use blockcopy when not array of primitives:
                for (int i = 0; i < shape[0]; ++i)
                {
                    var charArray = (char[])array.GetValue(i);
                    for (int j = 0; j < shape[1]; ++j)
                    {
                        bytes[index++] = (byte)charArray[j];
                    }
                }

                return bytes;
            }
            if (array.Rank == 2 && type == typeof(char))
            {
                var bytes = new byte[array.Length];
                Buffer.BlockCopy(array, 0, bytes, 0, array.Length);
                return bytes;
            }

            throw new NotImplementedException(String.Format("Char array with rank {0} and element type {1}", array.Rank, type));
        }
    }
}