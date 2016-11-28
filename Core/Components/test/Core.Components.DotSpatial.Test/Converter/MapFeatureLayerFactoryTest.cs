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
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapFeatureLayerFactoryTest
    {
        [Test]
        public void Create_MapPointData_ReturnMapPointLayer()
        {
            // Call
            IMapFeatureLayer layer = MapFeatureLayerFactory.Create(new MapPointData("test data")
            {
                Features = CreateTestData()
            });

            // Assert
            Assert.AreEqual(3, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPointLayer>(layer);
        }

        [Test]
        public void Create_MapLineData_ReturnMapLineLayer()
        {
            // Call
            IMapFeatureLayer layer = MapFeatureLayerFactory.Create(new MapLineData("test data")
            {
                Features = CreateTestData()
            });

            // Assert
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
        }

        [Test]
        public void Create_MapPolygonData_ReturnMapPolygonLayer()
        {
            // Call
            IMapFeatureLayer layer = MapFeatureLayerFactory.Create(new MapPolygonData("test data")
            {
                Features = CreateTestData()
            });

            // Assert
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
        }

        [Test]
        public void Create_OtherData_ThrownsNotSupportedException()
        {
            // Setup
            var testData = new TestMapData("test data");

            // Call
            TestDelegate test = () => MapFeatureLayerFactory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private static MapFeature[] CreateTestData()
        {
            return new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1.2, 3.4),
                            new Point2D(3.2, 3.4),
                            new Point2D(0.2, 2.4)
                        }
                    })
                })
            };
        }
    }
}