using System.Runtime.InteropServices;
using SharpMap.Data.Providers.EGIS.ShapeFileLib;

namespace EGIS.ShapeFileLib
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    unsafe struct ShapeFileMainHeader
    {
        internal const int MAIN_HEADER_LENGTH = 100;
        public int FileCode;
        public int UnusedByte1;
        public int UnusedByte2;
        public int UnusedByte3;
        public int UnusedByte4;
        public int UnusedByte5;
        public int FileLength;
        public int Version;
        public ShapeType ShapeType;
        public double Xmin;
        public double Ymin;
        public double Xmax;
        public double Ymax;
        public double Zmin;
        public double Zmax;
        public double Mmin;
        public double Mmax;	

        public ShapeFileMainHeader(byte[] data)
        {
            //first convert any BE ints in the data to LE
            //swap FileCode
            EndianUtils.SwapIntBytes(data,0);
            //no need to swap unused bytes
            //swap File Length
            EndianUtils.SwapIntBytes(data,24);
				
            fixed(byte* bPtr = data)
            {
                //now cast and dereference the pointer
                this = *(ShapeFileMainHeader*)bPtr;
            }
            //adjust FileLength to be number of bytes (not num words)
            FileLength*=2;            
        }

        public override string ToString()
        {
            string str = "Filecode = " + FileCode + ", FileLength = " + FileLength + ", Version = " + Version + ", ShapeType = " + ShapeType;
            str += ", XMin = " + Xmin + ", Ymin = " + Ymin + ", Xmax = " + Xmax + ", Ymax = " + Ymax + ", MMin = " + Mmin + ", Mmax = " + Mmax;
            return str;
        }

    }
}