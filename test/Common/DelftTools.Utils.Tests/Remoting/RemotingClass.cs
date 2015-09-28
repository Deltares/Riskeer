using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DelftTools.Utils.Tests.Remoting
{
    public class RemotingClass : IRemoteClass
    {
        public int ProcessId
        {
            get { return Process.GetCurrentProcess().Id; }
        }

        public void MethodThatThrowsException()
        {
            throw new NotSupportedException("nothing here");
        }

        public void CopyToHeap(Grid values)
        {
            Debug.WriteLine("CopyToHeap called");

            IntPtr intPtr;
            try
            {
                intPtr = Marshal.AllocHGlobal(values.gridValues.Length);
            }
            catch (OutOfMemoryException e)
            {
                throw new Exception("[OutOfMemoryException]", e);
            }
        }

        public float CopyArrayAndReturnLastValue(float[] array)
        {
            return array[array.Length - 1];
        }

        public void FreeHeap(IntPtr pointer)
        {
            Marshal.FreeHGlobal(pointer);
        }

        public string GetMemoryInfo()
        {
            var process = Process.GetCurrentProcess();
            var str = "";
            str += string.Format("Physical memory: {0:# ### ### ###} Kbytes\n", process.WorkingSet64 / 1000);
            str += string.Format("Private memory (heap): {0:# ### ### ###} Kbytes\n", process.PrivateMemorySize64 / 1000);
            str += string.Format("Virtual memory: {0:# ### ### ###} Kbytes\n", process.VirtualMemorySize64 / 1000);
            str += string.Format("Managed memory: {0:# ### ### ###} Kbytes\n", GC.GetTotalMemory(true) / 1000);

            return str;
        }
    }
}
