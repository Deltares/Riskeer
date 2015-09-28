using System;

namespace DelftTools.Utils.Tests.Remoting
{
    public interface IRemoteClass
    {
        int ProcessId { get; }

        void CopyToHeap(Grid values);

        float CopyArrayAndReturnLastValue(float[] array);

        void FreeHeap(IntPtr pointer);

        string GetMemoryInfo();

        void MethodThatThrowsException();
    }

    public class Grid
    {
        public byte[] gridValues;
    }
}