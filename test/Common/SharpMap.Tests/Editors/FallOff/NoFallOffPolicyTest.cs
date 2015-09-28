using System;
using System.Collections.Generic;
using System.Linq;
using DelftTools.TestUtils;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Utilities;
using NUnit.Framework;
using SharpMap.Editors.FallOff;
using Assert = NUnit.Framework.Assert;

namespace SharpMap.Tests.Editors.FallOff
{
    [TestFixture]
    public class NoFallOffPolicyTest
    {
        private static IGeometry lineStringSource;
        private static IGeometry lineStringTarget;
        private static IGeometry theLarch; // aka and now something completely different

        private static NoFallOffPolicy noFallOffPolicy;


        [SetUp]
        public void Setup()
        {
            noFallOffPolicy = new NoFallOffPolicy();
            lineStringSource =
                new LineString(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(20, 0),
                                       new Coordinate(30, 0), new Coordinate(40, 0)
                                   });
            lineStringTarget = (ILineString)lineStringSource.Clone();
            theLarch = new LineString(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(20, 0),
                                       new Coordinate(30, 0), new Coordinate(40, 0), new Coordinate(50, 0)
                                   });

        }

        [Test]
        public void SimpleMove1Coordinate()
        {
            noFallOffPolicy.Move(lineStringSource, 1, 5.0, 5.0);
            ICoordinate moveCoordinate = lineStringSource.Coordinates[1];
            Assert.AreEqual(15, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
        }

        [Test]
        public void SimpleMove2Coordinates()
        {
            noFallOffPolicy.Move(lineStringSource, null, new List<int>{1, 3}, 1, 5.0, 5.0);
            ICoordinate moveCoordinate = lineStringSource.Coordinates[1];
            Assert.AreEqual(15, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            moveCoordinate = lineStringSource.Coordinates[3];
            Assert.AreEqual(35, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
        }

        [Test]
        public void SimpleMoveWithSourceAndTargetGeometry()
        {
            noFallOffPolicy.Move(lineStringTarget, lineStringSource, 1, 5.0, 5.0);
            // The coordinates of the targetgeometry should be modified
            ICoordinate moveCoordinate = lineStringTarget.Coordinates[1];
            Assert.AreEqual(15, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            // The coordinates of the sourcegeometry should be untouched
            moveCoordinate = lineStringSource.Coordinates[1];
            Assert.AreEqual(10, moveCoordinate.X);
            Assert.AreEqual(0, moveCoordinate.Y);
        }

        [Test]
        public void MoveWithSourceAndTargetGeometryAnd2Coordinates()
        {
            noFallOffPolicy.Move(lineStringTarget, lineStringSource, null, new List<int>{1, 3}, 3, 5.0, 5.0);
            // The coordinates of the targetgeometry should be modified
            ICoordinate moveCoordinate = lineStringTarget.Coordinates[1];
            Assert.AreEqual(15, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            moveCoordinate = lineStringTarget.Coordinates[3];
            Assert.AreEqual(35, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            // The coordinates of the sourcegeometry should be untouched
            moveCoordinate = lineStringSource.Coordinates[1];
            Assert.AreEqual(10, moveCoordinate.X);
            Assert.AreEqual(0, moveCoordinate.Y);
            moveCoordinate = lineStringSource.Coordinates[3];
            Assert.AreEqual(30, moveCoordinate.X);
            Assert.AreEqual(0, moveCoordinate.Y);
        }
        [Test]
        public void FullTestWithTrackersGeometries()
        {
            IList<IGeometry> trackers = new List<IGeometry>();
            for (int i=0; i<lineStringTarget.Coordinates.Length; i++)
            {
                trackers.Add(new Point((ICoordinate)lineStringTarget.Coordinates[i].Clone()));
            }
            noFallOffPolicy.Move(lineStringTarget, lineStringSource, trackers, new List<int>{ 1, 3 }, 3, 5.0, 5.0);
            // only check for Trackers now; for other parameters see MoveWithSourceAndTargetGeometryAnd2Coordinates
            ICoordinate moveCoordinate = trackers[1].Coordinates[0];
            Assert.AreEqual(15, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
            moveCoordinate = trackers[3].Coordinates[0];
            Assert.AreEqual(35, moveCoordinate.X);
            Assert.AreEqual(5, moveCoordinate.Y);
        }

        [Test]
        [Category(TestCategory.Performance)]
        public void MovePolygonWithTrackersShouldBeFast()
        {
            var factory = new GeometricShapeFactory();
            factory.NumPoints = 10000; // many
            factory.Size = 300;
            var circle = factory.CreateCircle();
            var circleSource = (IGeometry) circle.Clone();

            var trackers = new List<IGeometry>();
            for (int i=0; i < circle.Coordinates.Length; i++)
            {
                trackers.Add(new Point((ICoordinate)circle.Coordinates[i].Clone()));
            }

            var handles = Enumerable.Range(0, trackers.Count).ToList();

            TestHelper.AssertIsFasterThan(40,
                                          () => noFallOffPolicy.Move(circle, circleSource, trackers, handles, 0, 5, 5));
        }

        /// <summary>
        /// Some failure tests
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveWithSourceAndDifferentTargetGeometry1()
        {
            noFallOffPolicy.Move(lineStringTarget, theLarch, 1, 5.0, 5.0);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveWithSourceAndDifferentTargetGeometry2()
        {
            noFallOffPolicy.Move(theLarch, lineStringSource, 1, 5.0, 5.0);
        }
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void SimpleMoveWithInvalidIndex()
        {
            noFallOffPolicy.Move(lineStringSource, -1, 5.0, 5.0);
        }
    }
}
