using System.IO;
using GeoAPI.Geometries;

namespace GisSharpBlog.NetTopologySuite.IO
{
    public class MsSqlSpatialWriter : WKBWriter
    {
        public override byte[] Write(IGeometry geometry)
        {
            byte[] bytes = new byte[SetByteStream(geometry)];
            Write(geometry, new MemoryStream(bytes));
            return bytes;
        }

        public override void Write(IGeometry geometry, Stream stream)
        {
            BinaryWriter writer = null;
            try
            {
                writer = encodingType == ByteOrder.LittleEndian ? new BinaryWriter(stream) : new BEBinaryWriter(stream);
                Write(geometry, writer);
                writer.Write(geometry.SRID);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        protected override int SetByteStream(IGeometry geometry)
        {
            return base.SetByteStream(geometry) + 4; // sizeof(int)
        }
    }
}