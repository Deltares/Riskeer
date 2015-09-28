using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Editors.FallOff;

namespace SharpMap.Tests.Editors.FallOff
{
    [TestFixture]
    public class LinearFallOffPolicyTest
    {
        private static IGeometry lineStringSource;
        private static IGeometry lineStringTarget;

        private static LinearFallOffPolicy linearFallOffPolicy;

        [SetUp]
        public void Setup()
        {
            linearFallOffPolicy = new LinearFallOffPolicy();
            lineStringSource =
                new LineString(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(20, 0),
                                       new Coordinate(30, 0), new Coordinate(40, 0)
                                   });
            lineStringTarget = (ILineString)lineStringSource.Clone();

        }
        /// <summary>
        /// Refer to NoFallOffPolicyTest for a simplified version of this test.
        /// </summary>
        [Test]
        public void SimpleMove1Coordinate()
        {
            linearFallOffPolicy.Move(lineStringSource, 1, 0, 5.0); // only change y
            ICoordinate moveCoordinate = lineStringSource.Coordinates[1];
            Assert.AreEqual(10, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            // For a linestring the start and end coordinates are ignored by the linear falloffpolicy
            moveCoordinate = lineStringSource.Coordinates[0];
            Assert.AreEqual(0, moveCoordinate.X);
            Assert.AreEqual(0, moveCoordinate.Y);
            moveCoordinate = lineStringSource.Coordinates[2];
            // x value unchanged
            Assert.AreEqual(20, moveCoordinate.X);
            // y value value is changed but the change should be less than 5
            Assert.Less(0, moveCoordinate.Y);
            Assert.Greater(5, moveCoordinate.Y);
        }

        [Test]
        public void SimpleMove2Coordinates()
        {
            linearFallOffPolicy.Move(lineStringSource, null, new List<int> { 1, 3 }, 1, 0.0, 5.0);
            ICoordinate moveCoordinate = lineStringSource.Coordinates[1];
            Assert.AreEqual(10, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            // For a linestring the start and end coordinates are ignored by the linear falloffpolicy
            moveCoordinate = lineStringSource.Coordinates[0];
            Assert.AreEqual(0, moveCoordinate.X);
            Assert.AreEqual(0, moveCoordinate.Y);
            moveCoordinate = lineStringSource.Coordinates[2];
            // x value unchanged
            Assert.AreEqual(20, moveCoordinate.X);
            // y value value is changed but the change should be less than 5
            Assert.Less(0, moveCoordinate.Y);
            Assert.Greater(5, moveCoordinate.Y);
            moveCoordinate = lineStringSource.Coordinates[4];
            Assert.AreEqual(40, moveCoordinate.X);
            Assert.AreEqual(0, moveCoordinate.Y);
        }
        [Test]
        public void FullTestWithTrackersGeometries()
        {
            IList<IGeometry> trackers = new List<IGeometry>();
            for (int i = 0; i < lineStringTarget.Coordinates.Length; i++)
            {
                trackers.Add(new Point((ICoordinate)lineStringTarget.Coordinates[i].Clone()));
            }
            linearFallOffPolicy.Move(lineStringTarget, lineStringSource, trackers, new List<int> { 2 }, 2, 0.0, 5.0);
            // Check coordinates
            Assert.AreEqual(0, lineStringTarget.Coordinates[0].X);
            Assert.AreEqual(0, lineStringTarget.Coordinates[0].Y);
            Assert.AreEqual(10, lineStringTarget.Coordinates[1].X);
            Assert.Less(0, lineStringTarget.Coordinates[1].Y);
            Assert.Greater(5, lineStringTarget.Coordinates[1].Y);
            Assert.AreEqual(20, lineStringTarget.Coordinates[2].X);
            Assert.AreEqual(5, lineStringTarget.Coordinates[2].Y);
            Assert.AreEqual(30, lineStringTarget.Coordinates[3].X);
            Assert.Less(0, lineStringTarget.Coordinates[3].Y);
            Assert.Greater(5, lineStringTarget.Coordinates[3].Y);
            Assert.AreEqual(40, lineStringTarget.Coordinates[4].X);
            Assert.AreEqual(0, lineStringTarget.Coordinates[4].Y);

            for (int i = 0; i < lineStringTarget.Coordinates.Length; i++)
            {
                Assert.AreEqual(lineStringTarget.Coordinates[i].X, trackers[i].Coordinates[0].X);
                Assert.AreEqual(lineStringTarget.Coordinates[i].Y, trackers[i].Coordinates[0].Y);
            }

        }
    }
}
