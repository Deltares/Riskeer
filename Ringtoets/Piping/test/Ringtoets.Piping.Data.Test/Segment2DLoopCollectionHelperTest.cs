using System;
using NUnit.Framework;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    public class Segment2DLoopCollectionHelperTest
    {
        [Test]
        public void CreateFromString_OnePoint_ReturnsExpectedPoints()
        {
            // Call
            var result = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "3",
                "..1..",
                ".....",
                "....."
                )).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(2, result[0].FirstPoint.X);
            Assert.AreEqual(2, result[0].FirstPoint.Y);
        }

        [Test]
        public void CreateFromString_TwoPoint_ReturnsExpectedPoints()
        {
            // Call
            var result = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "3",
                "..1..",
                ".....",
                "....2"
                )).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].FirstPoint.X);
            Assert.AreEqual(2, result[0].FirstPoint.Y);
            Assert.AreEqual(4, result[0].SecondPoint.X);
            Assert.AreEqual(0, result[0].SecondPoint.Y);
        }

        [Test]
        public void CreateFromString_TwoPointReversed_ReturnsExpectedPoints()
        {
            // Call
            var result = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "3",
                "..2..",
                ".....",
                "....1"
                )).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].SecondPoint.X);
            Assert.AreEqual(2, result[0].SecondPoint.Y);
            Assert.AreEqual(4, result[0].FirstPoint.X);
            Assert.AreEqual(0, result[0].FirstPoint.Y);
        }
    }
}