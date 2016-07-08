// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapPolygonDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPolygonData(null, "test data");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, string.Format("A feature collection is required when creating a subclass of {0}.", typeof(FeatureBasedMapData)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            // Call
            TestDelegate test = () => new MapPolygonData(features, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_EmptyPointsCollection_ThrowArgumentException()
        {
            // Setup
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(Enumerable.Empty<IEnumerable<Point2D>>())
                })
            };

            // Call
            TestDelegate call = () => new MapPolygonData(features, "Some invalid map data");

            // Assert
            string expectedMessage = "MapPolygonData only accept MapFeature instances whose MapGeometries contain a single point-collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewMapPolygonData()
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            // Call
            var data = new MapPolygonData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapPolygonData()
        {
            // Setup
            var features = new Collection<MapFeature> 
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        CreateOuterRingPoints()
                    })
                })
            };

            // Call
            var data = new MapPolygonData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
            CollectionAssert.AreEqual(new[]
            {
                CreateOuterRingPoints()
            }, data.Features.First().MapGeometries.First().PointCollections);
        }

        [Test]
        public void Constructor_WithOuterAndInnerRingPoints_CreatesNewMapPointData()
        {
            // Setup
            Point2D[] outerRingPoints = CreateOuterRingPoints();
            Point2D[] innerRing1Points = CreateInnerRingPoints(2.0, 4.0);
            Point2D[] innerRing2Points = CreateInnerRingPoints(8.0, 5.0);
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        outerRingPoints,
                        innerRing1Points,
                        innerRing2Points
                    })
                })
            };

            // Call
            var data = new MapPolygonData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);

            MapGeometry mapFeatureGeometry = data.Features.First().MapGeometries.First();
            var pointCollections = mapFeatureGeometry.PointCollections.ToArray();
            Assert.AreEqual(3, pointCollections.Length);
            CollectionAssert.AreEqual(outerRingPoints, pointCollections[0]);
            CollectionAssert.AreEqual(innerRing1Points, pointCollections[1]);
            CollectionAssert.AreEqual(innerRing2Points, pointCollections[2]);
        }

        [Test]
        public void Constructor_WithName_SetsName()
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };
            var name = "Some name";

            // Call
            var data = new MapPolygonData(features, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        private Point2D[] CreateOuterRingPoints()
        {
            return new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0),
                new Point2D(10.0, 10.0),
                new Point2D(0.0, 10.0),
                new Point2D(0.0, 0.0)
            };
        }

        private Point2D[] CreateInnerRingPoints(double lowerLeftCubeX, double upperRightCubeX)
        {
            return new[]
            {
                new Point2D(lowerLeftCubeX, lowerLeftCubeX),
                new Point2D(upperRightCubeX, lowerLeftCubeX),
                new Point2D(upperRightCubeX, upperRightCubeX),
                new Point2D(lowerLeftCubeX, upperRightCubeX),
                new Point2D(lowerLeftCubeX, lowerLeftCubeX)
            };
        }
    }
}