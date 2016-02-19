using System;

using Core.Common.Base.Geometry;
using Core.Common.TestUtil;

using NUnit.Framework;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class ReferenceLineTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var referenceLine = new ReferenceLine();

            // Assert
            CollectionAssert.IsEmpty(referenceLine.Points);
        }

        [Test]
        public void SetGeometry_ValidCoordinates_UpdatePoints()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            var newPoints = new[]
            {
                new Point2D(3, 5),
                new Point2D(1, 2)
            };

            // Call
            referenceLine.SetGeometry(newPoints);

            // Assert
            CollectionAssert.AreEqual(newPoints, referenceLine.Points);
            Assert.AreNotSame(newPoints, referenceLine.Points);
        }

        [Test]
        public void SetGeometry_NullGeometry_ThrowArgumentNullException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () =>referenceLine.SetGeometry(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "De geometrie die opgegeven werd voor de referentielijn heeft geen waarde.");
        }

        [Test]
        public void SetGeometry_CoordinatesHaveNullElement_ThrowArgumentException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            var pointWithNullElement = new[]
            {
                new Point2D(3, 5),
                null,
                new Point2D(1, 2)
            };

            // Call
            TestDelegate call = () => referenceLine.SetGeometry(pointWithNullElement);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Een punt in de geometrie voor de referentielijn heeft geen waarde.");
        }
    }
}