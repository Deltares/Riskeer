using System;

namespace DelftTools.Utils
{
    // YAGNI: remove these methods, use when needed: "Math.Abs(a - b) < epsilon"
    // What value to use for epsilon?
    public class Comparer
    {
        public static bool AlmostEqual2sComplement(float a, float b, int maxDeltaBits = 4)
        {
            if ((Single.IsNaN(a) && Single.IsNaN(b)) ||
                (Single.IsPositiveInfinity(a) && Single.IsPositiveInfinity(b)) ||
                (Single.IsNegativeInfinity(a) && Single.IsNegativeInfinity(b)))
            {
                return true;
            }

            if (Single.IsNaN(a) || Single.IsNaN(b) ||
                Single.IsPositiveInfinity(a) || Single.IsPositiveInfinity(b) ||
                Single.IsNegativeInfinity(a) || Single.IsNegativeInfinity(b))
            {
                return false;
            }

            try
            {
                int aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
                if (aInt < 0)
                {
                    aInt = Int32.MinValue - aInt; // Int32.MinValue = 0x80000000
                }

                int bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
                if (bInt < 0)
                {
                    bInt = Int32.MinValue - bInt;
                }

                int intDiff = Math.Abs(aInt - bInt);
                return intDiff <= (1 << maxDeltaBits);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool AlmostEqual2sComplement(double a, double b, int maxDeltaBits = 4)
        {
            if ((Double.IsNaN(a) && Double.IsNaN(b)) ||
                (Double.IsPositiveInfinity(a) && Double.IsPositiveInfinity(b)) ||
                (Double.IsNegativeInfinity(a) && Double.IsNegativeInfinity(b)))
            {
                return true;
            }

            if (Double.IsNaN(a) || Double.IsNaN(b) ||
                Double.IsPositiveInfinity(a) || Double.IsPositiveInfinity(b) ||
                Double.IsNegativeInfinity(a) || Double.IsNegativeInfinity(b))
            {
                return false;
            }

            try
            {
                Int64 aInt = BitConverter.ToInt64(BitConverter.GetBytes(a), 0);
                if (aInt < 0)
                {
                    aInt = Int64.MinValue - aInt;
                }

                Int64 bInt = BitConverter.ToInt64(BitConverter.GetBytes(b), 0);
                if (bInt < 0)
                {
                    bInt = Int64.MinValue - bInt;
                }

                Int64 intDiff = Math.Abs(aInt - bInt);
                return intDiff <= (1 << maxDeltaBits);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}