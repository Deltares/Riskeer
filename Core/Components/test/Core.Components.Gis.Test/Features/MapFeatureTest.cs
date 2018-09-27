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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Features
{
    [TestFixture]
    public class MapFeatureTest
    {
        [Test]
        public void ParameteredConstructor_WithMapGeometries_MapGeometriesAndDefaultValuesSet()
        {
            // Setup
            var mapGeometries = new[]
            {
                new MapGeometry(new[]
                {
                    Enumerable.Empty<Point2D>()
                })
            };

            // Call
            var mapFeature = new MapFeature(mapGeometries);

            // Assert
            CollectionAssert.IsEmpty(mapFeature.MetaData);
            CollectionAssert.AreEqual(mapGeometries, mapFeature.MapGeometries);
        }

        [Test]
        public void ParameteredConstructor_WithoutMapGeometries_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapFeature(null);

            // Assert
            const string expectedMessage = "MapFeature cannot be created without map geometries.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void MetaData_Always_ReturnsMetaData()
        {
            // Setup
            var mapFeature = new MapFeature(Enumerable.Empty<MapGeometry>());
            var testMetaData = new KeyValuePair<string, object>("test", new object());
            mapFeature.MetaData.Add(testMetaData);

            // Call
            IDictionary<string, object> featureMetaData = mapFeature.MetaData;

            // Assert
            Assert.AreEqual(1, featureMetaData.Count);
            Assert.IsTrue(featureMetaData.ContainsKey(testMetaData.Key));
            Assert.AreEqual(testMetaData.Value, featureMetaData[testMetaData.Key]);
        }
    }
}