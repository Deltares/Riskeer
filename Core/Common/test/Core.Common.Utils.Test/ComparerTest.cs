using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class ComparerTest
    {
        // In below tests, when a comparison using the == operator gives a different
        // result compared to AlmostEqual2sComplement, an assert is added (otherwise
        // the results are identical).
        [Test]
        public void CheckIfDoublesAreEqual()
        {
            double a = 5.0d;
            double b = 5.0d;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.0d;
            b = 0.0d;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.00000001d;
            b = 0.00000002d;
            Assert.IsFalse(Comparer.AlmostEqual2sComplement(a, b));

            a = 1.0d;
            b = 1.0d/7.0d;
            b *= 7.0d;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 1.0d;
            double inc = 1.0d/6.0d;
            b = inc + inc + inc + inc + inc + inc;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));
            Assert.IsFalse(a == b);

            a = 0.1d;
            b = 1.0d/10;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.9d;
            b = 0.3d + 0.6d;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));
            Assert.IsFalse(a == b);

            a = 0.1d;
            b = 0.05d + 0.05d;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.6d;
            b = 0.1d*6;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));
            Assert.IsFalse(a == b);
        }

        [Test]
        public void CheckIfFloatsAreEqual()
        {
            float a = 5.0f;
            float b = 5.0f;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.0f;
            b = 0.0f;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.00000001f;
            b = 0.00000002f;
            Assert.IsFalse(Comparer.AlmostEqual2sComplement(a, b));

            a = 1.0f;
            b = 1.0f/7.0f;
            b *= 7.0f;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 1.0f;
            float inc = 1.0f/6.0f;
            b = inc + inc + inc + inc + inc + inc;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.1f;
            b = 1.0f/10;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.9f;
            b = 0.3f + 0.6f;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));
            Assert.IsFalse(a == b);

            a = 0.1f;
            b = 0.05f + 0.05f;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));

            a = 0.6f;
            b = 0.1f*6;
            Assert.IsTrue(Comparer.AlmostEqual2sComplement(a, b));
        }
    }
}