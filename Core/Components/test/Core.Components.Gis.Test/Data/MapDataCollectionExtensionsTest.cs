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
using System.Linq;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataCollectionExtensionsTest
    {
        [Test]
        public void GetFeatureBasedMapDataRecursively_MapDataCollectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MapDataCollectionExtensions.GetFeatureBasedMapDataRecursively(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("mapDataCollection", exception.ParamName);
        }

        [Test]
        public void GetFeatureBasedMapDataRecursively_CollectionWithNestedData_ReturnAllFeatureBasedMapData()
        {
            // Setup            
            var line = new MapLineData("line");
            var polygon = new MapPolygonData("polygon");
            var nestedCollection = new MapDataCollection("nested");
            nestedCollection.Add(line);
            nestedCollection.Add(polygon);

            var collection = new MapDataCollection("test");
            var point = new MapPointData("point");
            collection.Add(point);
            collection.Add(nestedCollection);

            // Call
            FeatureBasedMapData[] featureBasedMapDatas = collection.GetFeatureBasedMapDataRecursively().ToArray();

            // Assert
            Assert.AreEqual(3, featureBasedMapDatas.Length);
            Assert.IsInstanceOf<MapPointData>(featureBasedMapDatas[0]);
            Assert.IsInstanceOf<MapLineData>(featureBasedMapDatas[1]);
            Assert.IsInstanceOf<MapPolygonData>(featureBasedMapDatas[2]);
        }
    }
}