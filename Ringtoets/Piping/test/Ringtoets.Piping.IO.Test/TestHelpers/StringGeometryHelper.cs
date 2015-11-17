using System;
using System.IO;
using System.Xml.Linq;

namespace Ringtoets.Piping.IO.Test.TestHelpers
{
    public static class StringGeometryHelper
    {
        public static XDocument GetXmlDocument(string str)
        {
            return XDocument.Load(new MemoryStream(GetByteArray(str)));
        }

        public static byte[] GetByteArray(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        } 
    }
}