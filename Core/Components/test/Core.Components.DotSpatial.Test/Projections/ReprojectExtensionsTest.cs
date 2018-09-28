// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Projections;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Projections
{
    [TestFixture]
    public class ReprojectExtensionsTest
    {
        [Test]
        public void Reproject_LinearRingNull_ThrowArgumentNullException()
        {
            // Setup
            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            ILinearRing linearRing = null;

            // Call
            TestDelegate call = () => linearRing.Reproject(projection, projection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("ring", paramName);
        }

        [Test]
        public void Reproject_ForLinearRingWithSourceProjectionNull_ThrowArgumentNullException()
        {
            // Setup
            var p1 = new Coordinate(0.0, 0.0);
            var p2 = new Coordinate(1.1, 1.1);

            var linearRing = new LinearRing(new[]
            {
                p1,
                p2,
                p1
            });

            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => linearRing.Reproject(null, projection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("source", paramName);
        }

        [Test]
        public void Reproject_ForLinearRingWithTargetProjectionNull_ThrowArgumentNullException()
        {
            // Setup
            var p1 = new Coordinate(0.0, 0.0);
            var p2 = new Coordinate(1.1, 1.1);

            var linearRing = new LinearRing(new[]
            {
                p1,
                p2,
                p1
            });

            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => linearRing.Reproject(projection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("target", paramName);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Reproject_ForLinearRingWithTooFewCoordinates_ThrowArgumentException(int numberOfPoints)
        {
            // Setup
            IEnumerable<Coordinate> coordinates = Enumerable.Range(0, numberOfPoints)
                                                            .Select(i => new Coordinate(i, i));
            var linearRing = new LinearRing(Enumerable.Empty<Coordinate>());
            foreach (Coordinate coordinate in coordinates)
            {
                linearRing.Coordinates.Add(coordinate);
            }

            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => linearRing.Reproject(projection, projection);

            // Assert
            const string message = "Ring must contain at least 3 coordinates.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("ring", paramName);
        }

        [Test]
        public void Reproject_ForLinearRingWithTargetAndSourceTheSameCoordinateSystem_DensifiedLinearRing()
        {
            // Setup
            var p1 = new Coordinate(0.0, 0.0);
            var p2 = new Coordinate(1.1, 1.1);
            var p3 = new Coordinate(2.2, 0.0);
            Coordinate[] triangleCoordinates =
            {
                p1,
                p2,
                p3,
                p1
            };
            var ring = new LinearRing(triangleCoordinates);

            ProjectionInfo sourceProjection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            ILinearRing reprojectedRing = ring.Reproject(sourceProjection, sourceProjection);

            // Assert
            const int numberOfEdges = 3;
            const int expectedNumberOfExtraPoints = 35;
            Assert.AreEqual(numberOfEdges * expectedNumberOfExtraPoints + numberOfEdges + 1, reprojectedRing.Coordinates.Count);

            const double allowedError = 1e-6; // Allow small drift in reprojecting to same coordinate system.

            const int expectedP1Index = 0;
            const int expectedP2Index = expectedNumberOfExtraPoints + 1;
            const int expectedP3Index = 2 * (expectedNumberOfExtraPoints + 1);
            const int expectedP1RepeatIndex = 3 * (expectedNumberOfExtraPoints + 1);

            AssertCoordinatesAreEqual(p1, reprojectedRing.Coordinates[expectedP1Index], allowedError);
            AssertCoordinatesAreEqual(GetExpectedExtraDesificationCoordinates(p1, p2, expectedNumberOfExtraPoints).ToArray(),
                                      TakeElementsFromTo(reprojectedRing.Coordinates, expectedP1Index + 1, expectedP2Index - 1).ToArray(),
                                      allowedError);
            AssertCoordinatesAreEqual(p2, reprojectedRing.Coordinates[expectedP2Index], allowedError);
            AssertCoordinatesAreEqual(GetExpectedExtraDesificationCoordinates(p2, p3, expectedNumberOfExtraPoints).ToArray(),
                                      TakeElementsFromTo(reprojectedRing.Coordinates, expectedP2Index + 1, expectedP3Index - 1).ToArray(),
                                      allowedError);
            AssertCoordinatesAreEqual(p3, reprojectedRing.Coordinates[expectedP3Index], allowedError);
            AssertCoordinatesAreEqual(GetExpectedExtraDesificationCoordinates(p3, p1, expectedNumberOfExtraPoints).ToArray(),
                                      TakeElementsFromTo(reprojectedRing.Coordinates, expectedP3Index + 1, expectedP1RepeatIndex - 1).ToArray(),
                                      allowedError);
            AssertCoordinatesAreEqual(p1, reprojectedRing.Coordinates[expectedP1RepeatIndex], allowedError);
        }

        [Test]
        public void Reproject_ForLinearRingWithDifferentCoordinateSystem_DensifiedAndReprojectedLinearRing()
        {
            // Setup
            var p1 = new Coordinate(0.0, 0.0);
            var p2 = new Coordinate(1.1, 1.1);
            var p3 = new Coordinate(2.2, 0.0);
            Coordinate[] triangleCoordinates =
            {
                p1,
                p2,
                p3,
                p1
            };
            var ring = new LinearRing(triangleCoordinates);

            ProjectionInfo sourceProjection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            ProjectionInfo targetProjection = KnownCoordinateSystems.Projected.World.WebMercator;

            // Call
            ILinearRing reprojectedRing = ring.Reproject(sourceProjection, targetProjection);

            // Assert
            const int numberOfEdges = 3;
            const int expectedNumberOfExtraPoints = 35;
            Assert.AreEqual(numberOfEdges * expectedNumberOfExtraPoints + numberOfEdges + 1, reprojectedRing.Coordinates.Count);

            const double allowedError = 1e-6;

            const int expectedP1Index = 0;
            const int expectedP2Index = expectedNumberOfExtraPoints + 1;
            const int expectedP3Index = 2 * (expectedNumberOfExtraPoints + 1);
            const int expectedP1RepeatIndex = 3 * (expectedNumberOfExtraPoints + 1);

            // Note: Very rough estimates can be gotten from https://epsg.io/transform#s_srs=28992&t_srs=900913
            //       Use them as sanity checks for the values below.
            AssertCoordinatesAreEqual(new Coordinate(368882.53051896818, 6102740.2091378355), reprojectedRing.Coordinates[expectedP1Index], allowedError);
            AssertCoordinatesAreEqual(new Coordinate(368884.12244735827, 6102741.8971153079), reprojectedRing.Coordinates[expectedP2Index], allowedError);
            AssertCoordinatesAreEqual(new Coordinate(368885.80535674578, 6102740.3003927628), reprojectedRing.Coordinates[expectedP3Index], allowedError);
            AssertCoordinatesAreEqual(reprojectedRing.Coordinates[expectedP1Index], reprojectedRing.Coordinates[expectedP1RepeatIndex], allowedError);
        }

        [Test]
        public void Reproject_ForLinearRingWithTargetCoordinateSystemWithoutTransform_ReturnDesifiedLinearRing()
        {
            // Setup
            var p1 = new Coordinate(0.0, 0.0);
            var p2 = new Coordinate(1.1, 1.1);
            var p3 = new Coordinate(2.2, 0.0);
            Coordinate[] triangleCoordinates =
            {
                p1,
                p2,
                p3,
                p1
            };
            var ring = new LinearRing(triangleCoordinates);

            ProjectionInfo sourceProjection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            var targetProjection = new ProjectionInfo();
            targetProjection.CopyProperties(sourceProjection);
            targetProjection.Transform = null;

            // Call
            ILinearRing reprojectedRing = ring.Reproject(sourceProjection, sourceProjection);

            // Assert
            const int numberOfEdges = 3;
            const int expectedNumberOfExtraPoints = 35;
            Assert.AreEqual(numberOfEdges * expectedNumberOfExtraPoints + numberOfEdges + 1, reprojectedRing.Coordinates.Count);

            const double allowedError = 1e-6; // Allow small drift in reprojecting to same coordinate system.

            const int expectedP1Index = 0;
            const int expectedP2Index = expectedNumberOfExtraPoints + 1;
            const int expectedP3Index = 2 * (expectedNumberOfExtraPoints + 1);
            const int expectedP1RepeatIndex = 3 * (expectedNumberOfExtraPoints + 1);

            AssertCoordinatesAreEqual(p1, reprojectedRing.Coordinates[expectedP1Index], allowedError);
            AssertCoordinatesAreEqual(GetExpectedExtraDesificationCoordinates(p1, p2, expectedNumberOfExtraPoints).ToArray(),
                                      TakeElementsFromTo(reprojectedRing.Coordinates, expectedP1Index + 1, expectedP2Index - 1).ToArray(),
                                      allowedError);
            AssertCoordinatesAreEqual(p2, reprojectedRing.Coordinates[expectedP2Index], allowedError);
            AssertCoordinatesAreEqual(GetExpectedExtraDesificationCoordinates(p2, p3, expectedNumberOfExtraPoints).ToArray(),
                                      TakeElementsFromTo(reprojectedRing.Coordinates, expectedP2Index + 1, expectedP3Index - 1).ToArray(),
                                      allowedError);
            AssertCoordinatesAreEqual(p3, reprojectedRing.Coordinates[expectedP3Index], allowedError);
            AssertCoordinatesAreEqual(GetExpectedExtraDesificationCoordinates(p3, p1, expectedNumberOfExtraPoints).ToArray(),
                                      TakeElementsFromTo(reprojectedRing.Coordinates, expectedP3Index + 1, expectedP1RepeatIndex - 1).ToArray(),
                                      allowedError);
            AssertCoordinatesAreEqual(p1, reprojectedRing.Coordinates[expectedP1RepeatIndex], allowedError);
        }

        [Test]
        public void Reproject_ForNullExtent_ThrowArgumentNullException()
        {
            // Setup
            Extent extent = null;

            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => extent.Reproject(projection, projection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("extent", paramName);
        }

        [Test]
        public void Reproject_ForExtentWithSourceProjectionNull_ThrowArgumentNullException()
        {
            // Setup
            var extent = new Extent();

            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => extent.Reproject(null, projection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("source", paramName);
        }

        [Test]
        public void Reproject_ForExtentWithTargetProjectionNull_ThrowArgumentNullException()
        {
            // Setup
            var extent = new Extent();

            ProjectionInfo projection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            TestDelegate call = () => extent.Reproject(projection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("target", paramName);
        }

        [Test]
        public void Reproject_ForExtentWithTargetAndSourceTheSameCoordinateSystem_ReturnEqualExtent()
        {
            // Setup
            var extent = new Extent(1.1, 2.2, 3.3, 4.4);

            ProjectionInfo sourceProjection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            // Call
            Extent result = extent.Reproject(sourceProjection, sourceProjection);

            // Assert
            AssertExtentAreEqual(extent, result, 1e-6);
        }

        [Test]
        public void Reproject_ForExtentWithDifferentCoordinateSystems_ReturnReprojectedExtent()
        {
            // Setup
            var extent = new Extent(1.1, 2.2, 3.3, 4.4);

            ProjectionInfo source = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
            ProjectionInfo targetProjection = KnownCoordinateSystems.Projected.World.WebMercator;

            // Call
            Extent result = extent.Reproject(source, targetProjection);

            // Assert
            var expectedExtent = new Extent(368883.9859757676, 6102743.5394654693, 368887.35179595748, 6102746.9154212056);
            AssertExtentAreEqual(expectedExtent, result, 1e-6);
        }

        [Test]
        public void Reproject_ForExtentWithTargetProjectionWithoutTransform_ReturnEqualExtent()
        {
            // Setup
            var extent = new Extent(1.1, 2.2, 3.3, 4.4);

            ProjectionInfo source = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;

            var targetProjection = new ProjectionInfo();
            targetProjection.CopyProperties(KnownCoordinateSystems.Projected.World.WebMercator);
            targetProjection.Transform = null;

            // Call
            Extent result = extent.Reproject(source, targetProjection);

            // Assert
            AssertExtentAreEqual(extent, result, 1e-6);
        }

        private void AssertExtentAreEqual(Extent expected, Extent actual, double delta)
        {
            Assert.AreEqual(expected.MinX, actual.MinX, delta);
            Assert.AreEqual(expected.MinY, actual.MinY, delta);
            Assert.AreEqual(expected.MaxX, actual.MaxX, delta);
            Assert.AreEqual(expected.MaxY, actual.MaxY, delta);
        }

        private static IEnumerable<Coordinate> TakeElementsFromTo(IEnumerable<Coordinate> coordinates, int fromIndex, int toIndex)
        {
            return coordinates.Skip(fromIndex).Take(toIndex - fromIndex + 1);
        }

        private void AssertCoordinatesAreEqual(Coordinate expected, Coordinate actual, double delta)
        {
            Assert.AreEqual(expected.X, actual.X, delta);
            Assert.AreEqual(expected.Y, actual.Y, delta);
        }

        private void AssertCoordinatesAreEqual(IList<Coordinate> expectedCoordinates, IList<Coordinate> actualCoordinates, double delta)
        {
            Assert.AreEqual(expectedCoordinates.Count, actualCoordinates.Count);
            for (var i = 0; i < expectedCoordinates.Count; i++)
            {
                AssertCoordinatesAreEqual(expectedCoordinates[i], actualCoordinates[i], delta);
            }
        }

        private IEnumerable<Coordinate> GetExpectedExtraDesificationCoordinates(Coordinate start, Coordinate end, int expectedNumberOfAdditionalPoints)
        {
            double dx = (end.X - start.X) / (expectedNumberOfAdditionalPoints + 1);
            double dy = (end.Y - start.Y) / (expectedNumberOfAdditionalPoints + 1);

            for (var i = 1; i <= expectedNumberOfAdditionalPoints; i++)
            {
                yield return new Coordinate(start.X + i * dx, start.Y + i * dy);
            }
        }
    }
}