// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Components.DotSpatial.Layer;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Layer
{
    [TestFixture]
    public class FeatureBasedMapDataLayerFactoryTest
    {
        [Test]
        public void Create_MapPointData_ReturnMapPointDataLayer()
        {
            // Call
            IFeatureBasedMapDataLayer layer = FeatureBasedMapDataLayerFactory.Create(new MapPointData("test data"));

            // Assert
            Assert.IsInstanceOf<MapPointDataLayer>(layer);
        }

        [Test]
        public void Create_MapLineData_ReturnMapLineDataLayer()
        {
            // Call
            IFeatureBasedMapDataLayer layer = FeatureBasedMapDataLayerFactory.Create(new MapLineData("test data"));

            // Assert
            Assert.IsInstanceOf<MapLineDataLayer>(layer);
        }

        [Test]
        public void Create_MapPolygonData_ReturnMapPolygonDataLayer()
        {
            // Call
            IFeatureBasedMapDataLayer layer = FeatureBasedMapDataLayerFactory.Create(new MapPolygonData("test data"));

            // Assert
            Assert.IsInstanceOf<MapPolygonDataLayer>(layer);
        }

        [Test]
        public void Create_OtherData_ThrowsNotSupportedException()
        {
            // Setup
            var testData = new TestFeatureBasedMapData("test data");

            // Call
            TestDelegate test = () => FeatureBasedMapDataLayerFactory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }
    }
}