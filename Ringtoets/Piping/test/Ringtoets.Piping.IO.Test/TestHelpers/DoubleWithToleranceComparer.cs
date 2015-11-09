using System;
using System.Collections;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Test.TestHelpers
{
    public class DoubleWithToleranceComparer : IComparer
    {
        private readonly double tolerance;

        public DoubleWithToleranceComparer(double tolerance)
        {
            this.tolerance = tolerance;
        }

        private int Compare(double firstDouble, double secondDouble)
        {
            var diff = firstDouble - secondDouble;

            var tolerable = Math.Abs(diff) < tolerance;

            var nonTolerableDiff = !tolerable && diff < 0 ? -1 : 1;

            return tolerable ? 0 : nonTolerableDiff;
        }

        public int Compare(object x, object y)
        {
            if (!(x is double) || !(y is double))
            {
                throw new NotSupportedException(string.Format("Cannot compare objects other than {0} with this comparer.", typeof(Point2D)));
            }
            return Compare((double)x, (double)y);
        }
    }
}