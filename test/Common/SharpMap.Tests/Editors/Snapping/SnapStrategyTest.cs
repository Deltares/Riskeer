using System;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.Editors.Snapping;
using SharpMap.Layers;

namespace SharpMap.Tests.Editors.Snapping
{
    [TestFixture]
    public class SnapStrategyTest
    {
        private static IGeometry lineString;
        private static IGeometry multiLineString;

        [SetUp]
        public void Setup()
        {
            lineString =
                new LineString(new[]
                {
                    new Coordinate(0, 0),
                    new Coordinate(10, 0),
                    new Coordinate(20, 0),
                    new Coordinate(30, 0),
                    new Coordinate(40, 0)
                });
            var lineString2 =
                new LineString(new[]
                {
                    new Coordinate(0, 10),
                    new Coordinate(10, 10),
                    new Coordinate(20, 10),
                    new Coordinate(30, 10),
                    new Coordinate(40, 10)
                });
            multiLineString =
                new MultiLineString(new[]
                {
                    (ILineString) lineString,
                    lineString2
                });
        }

        [Test]
        public void SnapToLineStringUsingFreeAtObject()
        {
            var mockRepository = new MockRepository();
            var feature = mockRepository.CreateMock<IFeature>();

            using (mockRepository.Record())
            {
                Expect.Call(feature.Geometry).Return(lineString).Repeat.Any();
            }

            using (mockRepository.Playback())
            {
                var candidates = new[]
                {
                    new Tuple<IFeature, ILayer>(feature, new VectorLayer())
                };
                IPoint snapSource = new Point(5, 5);
                SnapRule snapRule = new SnapRule
                {
                    SnapRole = SnapRole.FreeAtObject,
                    Obligatory = true,
                    PixelGravity = 40
                };

                SnapResult snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                                          snapSource.Coordinates[0],
                                                          CreateEnvelope(snapSource, 10),
                                                          0);
                Assert.IsNotNull(snapResults);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(0, snapResults.Location.Y);
                Assert.AreEqual(5, snapResults.Location.X);
            }
        }

        [Test]
        public void SnapToLineStringUsingDifferentRoles()
        {
            MockRepository mockRepository = new MockRepository();
            IFeature feature = mockRepository.CreateMock<IFeature>();

            using (mockRepository.Record())
            {
                Expect.Call(feature.Geometry).Return(lineString).Repeat.Any();
            }

            using (mockRepository.Playback())
            {
                var candidates = new[]
                {
                    new Tuple<IFeature, ILayer>(feature, new VectorLayer())
                };

                SnapRule snapRule = new SnapRule
                {
                    Obligatory = true,
                    PixelGravity = 40
                };

                IPoint snapSource = new Point(6, 5);

                snapRule.SnapRole = SnapRole.AllTrackers;
                SnapResult snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                                          snapSource.Coordinates[0],
                                                          CreateEnvelope(snapSource, 10),
                                                          0);
                Assert.IsNotNull(snapResults);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(0, snapResults.Location.Y);
                Assert.AreEqual(10, snapResults.Location.X);

                snapRule.SnapRole = SnapRole.Start;
                snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                               snapSource.Coordinates[0],
                                               CreateEnvelope(snapSource, 10),
                                               0);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(0, snapResults.Location.Y);
                Assert.AreEqual(0, snapResults.Location.X);

                snapSource = new Point(6, 75);
                snapRule.SnapRole = SnapRole.End;
                snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                               snapSource.Coordinates[0],
                                               CreateEnvelope(snapSource, 10),
                                               0);
                Assert.IsNull(snapResults.SnappedFeature);

                snapSource = new Point(6, 5);
                snapRule.SnapRole = SnapRole.Free;
                snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                               snapSource.Coordinates[0],
                                               CreateEnvelope(snapSource, 10),
                                               0);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(5, snapResults.Location.Y);
                Assert.AreEqual(6, snapResults.Location.X);
            }
        }

        [Test]
        public void SnapToMultiLineStringUsingDifferentRoles()
        {
            MockRepository mockRepository = new MockRepository();
            IFeature feature = mockRepository.CreateMock<IFeature>();

            using (mockRepository.Record())
            {
                Expect.Call(feature.Geometry).Return(multiLineString).Repeat.Any();
            }

            using (mockRepository.Playback())
            {
                var candidates = new[]
                {
                    new Tuple<IFeature, ILayer>(feature, new VectorLayer())
                };

                SnapRule snapRule = new SnapRule
                {
                    Obligatory = true,
                    PixelGravity = 40
                };

                IPoint snapSource = new Point(6, 5);

                snapRule.SnapRole = SnapRole.AllTrackers;
                SnapResult snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                                          snapSource.Coordinates[0],
                                                          CreateEnvelope(snapSource, 10),
                                                          0);
                Assert.IsNotNull(snapResults);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(0, snapResults.Location.Y);
                Assert.AreEqual(10, snapResults.Location.X);

                snapRule.SnapRole = SnapRole.Start;
                snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                               snapSource.Coordinates[0],
                                               CreateEnvelope(snapSource, 10),
                                               0);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(0, snapResults.Location.Y);
                Assert.AreEqual(0, snapResults.Location.X);

                snapSource = new Point(6, 75);
                snapRule.SnapRole = SnapRole.End;
                snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                               snapSource.Coordinates[0],
                                               CreateEnvelope(snapSource, 10),
                                               0);
                Assert.IsNull(snapResults.SnappedFeature);

                snapSource = new Point(6, 5);
                snapRule.SnapRole = SnapRole.Free;
                snapResults = snapRule.Execute(null, candidates, snapSource, null,
                                               snapSource.Coordinates[0],
                                               CreateEnvelope(snapSource, 10),
                                               0);
                Assert.IsNotNull(snapResults.Location);
                Assert.AreEqual(5, snapResults.Location.Y);
                Assert.AreEqual(6, snapResults.Location.X);
            }
        }

        private static IEnvelope CreateEnvelope(IPoint point, double marge)
        {
            return new Envelope(point.Coordinates[0].X - marge, point.Coordinates[0].X + marge,
                                point.Coordinates[0].Y - marge, point.Coordinates[0].Y + marge);
        }
    }
}