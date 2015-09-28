using System.Linq;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Editors.FallOff;

namespace SharpMap.Tests.Editors.FallOff
{
    [TestFixture]
    public class RingFallOffPolicyTest
    {
        private static IGeometry lineStringSource;
        private static IGeometry lineStringTarget;

        private static RingFallOffPolicy ringFallOffPolicy;

        [SetUp]
        public void Setup()
        {
            ringFallOffPolicy = new RingFallOffPolicy();
            lineStringSource =
                new LineString(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(20, 0),
                                       new Coordinate(30, 0), new Coordinate(40, 0), new Coordinate(0, 0)
                                   });
            lineStringTarget = (ILineString)lineStringSource.Clone();
        }

        /// <summary>
        /// Refer to NoFallOffPolicyTest for a simplified version of this test.
        /// </summary>
        [Test]
        public void SimpleMoveFirstCoordinate()
        {
            ringFallOffPolicy.Move(lineStringSource, 0, 0, 5.0); // only change y
            ICoordinate firstCoordinate = lineStringSource.Coordinates[0];
            ICoordinate lastCoordinate = lineStringSource.Coordinates[lineStringSource.Coordinates.Length-1];
            Assert.AreEqual(0, firstCoordinate.X);
            Assert.AreEqual(5, firstCoordinate.Y);
            Assert.AreEqual(0, lastCoordinate.X);
            Assert.AreEqual(5, lastCoordinate.Y);
        }

        /// <summary>
        /// Refer to NoFallOffPolicyTest for a simplified version of this test.
        /// </summary>
        [Test]
        public void SimpleMoveLastCoordinate()
        {
            ringFallOffPolicy.Move(lineStringSource, lineStringSource.Coordinates.Length-1, 0, 5.0); // only change y
            ICoordinate firstCoordinate = lineStringSource.Coordinates[0];
            ICoordinate lastCoordinate = lineStringSource.Coordinates[lineStringSource.Coordinates.Length - 1];
            Assert.AreEqual(0, firstCoordinate.X);
            Assert.AreEqual(5, firstCoordinate.Y);
            Assert.AreEqual(0, lastCoordinate.X);
            Assert.AreEqual(5, lastCoordinate.Y);
        }

        [Test]
        public void SimpleMoveAllCoordinates()
        {
            ringFallOffPolicy.Move(lineStringSource, null,
                                   Enumerable.Range(0, lineStringSource.Coordinates.Length).ToList(), 
                                   1, 0.0, 5.0);
            ICoordinate firstCoordinate = lineStringSource.Coordinates[0];
            ICoordinate lastCoordinate = lineStringSource.Coordinates[lineStringSource.Coordinates.Length - 1];
            Assert.AreEqual(lastCoordinate.X, firstCoordinate.X);
            Assert.AreEqual(lastCoordinate.Y, firstCoordinate.Y);
        }
    }
}
