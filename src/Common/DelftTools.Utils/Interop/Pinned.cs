using System;
using System.Runtime.InteropServices;

namespace DelftTools.Utils.Interop
{
    public class Pinned : IDisposable
    {
        private readonly IntPtr intPtr;
        private GCHandle handle;

        public Pinned(object o)
        {
            handle = GCHandle.Alloc(o, GCHandleType.Pinned);
            intPtr = handle.AddrOfPinnedObject();
        }

        public IntPtr IntPtr
        {
            get
            {
                return intPtr;
            }
        }

        public static implicit operator IntPtr(Pinned pinner)
        {
            return pinner.intPtr;
        }

        public void Dispose()
        {
            handle.Free();
        }
    }
}