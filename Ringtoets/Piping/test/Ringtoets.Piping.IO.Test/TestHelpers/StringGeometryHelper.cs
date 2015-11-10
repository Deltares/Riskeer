using System;

namespace Ringtoets.Piping.IO.Test.TestHelpers
{
    public class StringGeometryHelper
    {
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        } 
    }
}