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
using System.Linq;
using Core.Components.Chart.Data;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Data
{
    [TestFixture]
    public class ChartDataCollectionExtensionsTest
    {
        [Test]
        public void GetFeatureBasedChartDataRecursively_ChartDataCollectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ChartDataCollectionExtensions.GetChartDataRecursively(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("chartDataCollection", exception.ParamName);
        }

        [Test]
        public void GetFeatureBasedChartDataRecursively_CollectionWithNestedData_ReturnAllFeatureBasedChartData()
        {
            // Setup            
            var line = new ChartLineData("line");
            var polygon = new ChartAreaData("polygon");
            var nestedCollection = new ChartDataCollection("nested");
            nestedCollection.Add(line);
            nestedCollection.Add(polygon);

            var collection = new ChartDataCollection("test");
            var point = new ChartPointData("point");
            collection.Add(point);
            collection.Add(nestedCollection);

            // Call
            ChartData[] featureBasedChartDatas = collection.GetChartDataRecursively().ToArray();

            // Assert
            Assert.AreEqual(3, featureBasedChartDatas.Length);
            Assert.IsInstanceOf<ChartPointData>(featureBasedChartDatas[0]);
            Assert.IsInstanceOf<ChartLineData>(featureBasedChartDatas[1]);
            Assert.IsInstanceOf<ChartAreaData>(featureBasedChartDatas[2]);
        }
    }
}