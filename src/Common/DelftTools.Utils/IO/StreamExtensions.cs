using System.IO;

namespace DelftTools.Utils.IO
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream resourceStream)
        {
            var buffer = new byte[resourceStream.Length];
            resourceStream.Read(buffer, 0, (int)resourceStream.Length);
            return buffer;
        }
        
    }
}