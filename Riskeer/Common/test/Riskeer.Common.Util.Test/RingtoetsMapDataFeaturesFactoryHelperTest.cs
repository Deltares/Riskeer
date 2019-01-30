// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class RingtoetsMapDataFeaturesFactoryHelperTest
    {
        [Test]
        public void CreateSinglePointMapFeature_PointNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("point", exception.ParamName);
        }

        [Test]
        public void CreateSinglePointMapFeature_WithPoint_CreatesASinglePointMapFeature()
        {
            // Setup
            var point = new Point2D(0, 0);

            // Call
            MapFeature pointMapFeature = RingtoetsMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(point);

            // Assert
            MapGeometry[] mapGeometries = pointMapFeature.MapGeometries.ToArray();
            Assert.AreEqual(1, mapGeometries.Length);
            MapGeometry mapGeometry = mapGeometries.First();
            IEnumerable<Point2D>[] geometryPointCollections = mapGeometry.PointCollections.ToArray();
            Assert.AreEqual(1, geometryPointCollections.Length);
            Assert.AreSame(point, geometryPointCollections.First().First());
        }
    }
}