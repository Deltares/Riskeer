// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DelftTools.Utils.NetCdf
{
    internal class Utf8Marshaler : ICustomMarshaler
    {
        /// <summary>Value of argument being marshalled if it is a StringBuilder</summary>
        private StringBuilder sb = null;

        [ThreadStatic]
        private static Dictionary<IntPtr, int> arrayPointers = null;

        private static Dictionary<IntPtr, int> ArrayPointers
        {
            get
            {
                if (arrayPointers == null)
                    arrayPointers = new Dictionary<IntPtr, int>();
                return arrayPointers;
            }
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            // Nothing to do here
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (arrayPointers != null && arrayPointers.ContainsKey(pNativeData))
            {
                int n = arrayPointers[pNativeData];
                int sz = Marshal.SizeOf(typeof(byte*));
                for (int i = 0; i < n; i++)
                {
                    IntPtr ptr = Marshal.ReadIntPtr(pNativeData, sz * i);
                    Marshal.FreeHGlobal(ptr);
                }
                arrayPointers.Remove(pNativeData);
            }
            Marshal.FreeHGlobal(pNativeData); // Originally it was Marshal.Release!
        }

        public int GetNativeDataSize()
        {
            return Marshal.SizeOf(typeof(byte));
        }

        public int GetNativeDataSize(IntPtr ptr)
        {
            int size = 0;
            for (size = 0; Marshal.ReadByte(ptr, size) != 0; size++)
                ;
            return size + 1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
                return IntPtr.Zero;
            Type type = ManagedObj.GetType();
            string str;
            int capacity = 0;
            if (type == typeof(StringBuilder))
            {
                sb = ((StringBuilder)ManagedObj);
                str = sb.ToString();
                capacity = sb.Capacity;
            }
            else if (type == typeof(string))
            {
                str = (string)ManagedObj;
            }
            else if (type == typeof(string[]))
            {
                string[] arr = (string[])ManagedObj;
                int n = arr.Length;
                int sz = Marshal.SizeOf(typeof(byte*));
                IntPtr ptrA = Marshal.AllocHGlobal(sz * n);
                for (int i = 0; i < n; i++)
                {
                    Marshal.WriteIntPtr(ptrA, sz * i, MarshalManagedToNative(arr[i]));
                }
                ArrayPointers[ptrA] = n;
                return ptrA;
            }
            else
                throw new ArgumentException("ManagedObj", "Can only marshal type of System.String");

            byte[] array = Encoding.UTF8.GetBytes(str);
            int size = Marshal.SizeOf(typeof(byte)) * (Math.Max(array.Length, capacity) + 1);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(array, 0, ptr, array.Length);
            Marshal.WriteByte(ptr, array.Length, 0);

            return ptr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
                return null;
            int size = GetNativeDataSize(pNativeData);
            byte[] array = new byte[size - 1];
            Marshal.Copy(pNativeData, array, 0, size - 1);
            string res = Encoding.UTF8.GetString(array);

            if (sb != null)
            {
                sb.Length = 0;
                sb.Append(res);
                return sb;
            }
            return res;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (cookie == "DontAppendZero")
                return UTF8MarshalerNoZero.Instance;
            return new Utf8Marshaler();
        }
    }

    internal class UTF8MarshalerNoZero : ICustomMarshaler
    {
        private static UTF8MarshalerNoZero instance;

        public static UTF8MarshalerNoZero Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UTF8MarshalerNoZero();
                }
                return instance;
            }
        }


        public void CleanUpManagedData(object ManagedObj)
        {
            // Nothing to do here
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
                return IntPtr.Zero;
            Type type = ManagedObj.GetType();
            string str;
            if (type == typeof(string))
            {
                str = (string)ManagedObj;
            }
            else
                throw new ArgumentException("ManagedObj", "Can only marshal type of System.String");

            byte[] array = Encoding.UTF8.GetBytes(str);
            int size = Marshal.SizeOf(typeof(byte)) * array.Length;

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(array, 0, ptr, array.Length);

            return ptr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            throw new NotSupportedException();
        }

        public int GetNativeDataSize()
        {
            return Marshal.SizeOf(typeof(byte));
        }
    }

    internal static class UTF8Marshal
    {
        public static string PtrToStringUTF8(IntPtr intPtr)
        {
            if (intPtr == IntPtr.Zero)
                return null;
            int size = GetNativeDataSize(intPtr);
            byte[] array = new byte[size - 1];
            Marshal.Copy(intPtr, array, 0, size - 1);
            string res = Encoding.UTF8.GetString(array);
            return res;
        }

        public static int GetNativeDataSize(IntPtr ptr)
        {
            int size = 0;
            for (size = 0; Marshal.ReadByte(ptr, size) != 0; size++)
                ;
            return size + 1;
        }

        public static string PtrToStringUTF8(IntPtr ptr, int len)
        {
            if (ptr == IntPtr.Zero)
                return null;
            byte[] array = new byte[len];
            Marshal.Copy(ptr, array, 0, len);
            string res = Encoding.UTF8.GetString(array);
            return res;
        }

        public static IntPtr StringUTF8ToPtr(string str)
        {
            byte[] array = Encoding.UTF8.GetBytes(str);
            return BytesUTF8ToPtr(array);
        }

        public static IntPtr BytesUTF8ToPtr(byte[] array)
        {
            int size = Marshal.SizeOf(typeof(byte)) * array.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(array, 0, ptr, array.Length);
            return ptr;
        }

    }
}

