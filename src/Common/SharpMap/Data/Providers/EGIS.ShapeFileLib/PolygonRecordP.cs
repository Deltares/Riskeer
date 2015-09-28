using System.Runtime.InteropServices;

namespace SharpMap.Data.Providers.EGIS.ShapeFileLib
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct PolygonRecordP
    {
        public ShapeType ShapeType;        
        public Box bounds;
        public int NumParts;
        public int NumPoints;
        public fixed int PartOffsets[1];

        public int DataOffset
        {
            get
            {
                return 44 + (NumParts << 2);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Box
    {
        internal double xmin;
        internal double ymin;
        internal double xmax;
        internal double ymax;
    }

}