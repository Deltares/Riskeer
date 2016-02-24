using System;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.TestUtil;

using NUnit.Framework;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var expectedName = "<Name>";
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            // Call
            var section = new FailureMechanismSection(expectedName, points);

            // Assert
            Assert.AreEqual(expectedName, section.Name);
            Assert.AreNotSame(points, section.Points);
            CollectionAssert.AreEqual(points, section.Points);
        }

        [Test]
        public void Constructor_NameIsNull_ThrowArugmentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection(null, Enumerable.Empty<Point2D>());

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Constructor_GeometryPointsIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection("name", null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Constructor_GeometryPointsContainsNullElement_ThrowArgumentException()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                null,
                new Point2D(3, 4)
            };

            // Call
            TestDelegate call = () => new FailureMechanismSection("name", geometryPoints);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "One or multiple elements are null.");
        }

        [Test]
        public void Constructor_GeometryIsEmpty_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection("", Enumerable.Empty<Point2D>());

            // Assert
            string expectedMessage = "Vak moet minstens uit 1 punt bestaan.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void GetStart_SectionWithPoints_ReturnFirstPoint()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };
            var section = new FailureMechanismSection("A", geometryPoints);

            // Call
            Point2D startingPoint = section.GetStart();

            // Assert
            Assert.AreEqual(geometryPoints[0], startingPoint);
        }

        [Test]
        public void GetEnd_SectionWithPoints_ReturnLastPoint()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };
            var section = new FailureMechanismSection("A", geometryPoints);

            // Call
            Point2D endingPoint = section.GetLast();

            // Assert
            Assert.AreEqual(geometryPoints[1], endingPoint);
        }
    }
}