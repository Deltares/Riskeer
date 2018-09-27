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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Geometries
{
    [TestFixture]
    public class MapGeometryTest
    {
        [Test]
        public void ParameteredConstructor_WithPoints_PointsSet()
        {
            // Setup
            var list1 = new List<Point2D>
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4),
                new Point2D(5.5, 6.6)
            };

            var list2 = new List<Point2D>
            {
                new Point2D(7.7, 8.8),
                new Point2D(9.9, 10.1),
                new Point2D(11.11, 12.12)
            };

            var list3 = new List<Point2D>
            {
                new Point2D(13.13, 14.14),
                new Point2D(15.15, 16.16),
                new Point2D(17.17, 18.18)
            };

            var geometriesList = new List<IEnumerable<Point2D>>
            {
                list1,
                list2,
                list3
            };

            // Call
            var mapGeometry = new MapGeometry(geometriesList);

            // Assert
            CollectionAssert.AreEqual(geometriesList, mapGeometry.PointCollections);
        }

        [Test]
        public void ParameteredConstructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapGeometry(null);

            // Assert
            const string expectedMessage = "MapGeometry cannot be created without points.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }
    }
}