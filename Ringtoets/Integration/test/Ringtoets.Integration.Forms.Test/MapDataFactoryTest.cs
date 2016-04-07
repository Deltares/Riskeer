// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class MapDataFactoryTest
    {
        [Test]
        public void CreateEmptyLineData_Always_ReturnEmptyMapLineDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            MapLineData mapData = MapDataFactory.CreateEmptyLineData(name);

            // Assert
            Assert.AreEqual(name, mapData.Name);
            Assert.IsEmpty(mapData.Features);
        }

        [Test]
        public void CreateEmptyPointData_Always_ReturnEmptyMapLineDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            MapPointData mapData = MapDataFactory.CreateEmptyPointData(name);

            // Assert
            Assert.AreEqual(name, mapData.Name);
            Assert.IsEmpty(mapData.Features);
        }

        [Test]
        public void Create_GivenReferenceLine_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var line = new ReferenceLine();
            line.SetGeometry(pointsOne);

            // Call
            MapData data = MapDataFactory.Create(line);

            // Assert
            Assert.IsInstanceOf<MapLineData>(data);
            var mapLineData = (MapLineData)data;
            Assert.AreEqual(1, mapLineData.Features.Count());
            Assert.AreEqual(1, mapLineData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, mapLineData.Features.ElementAt(0).MapGeometries.ElementAt(0));

            Assert.AreEqual(Common.Data.Properties.Resources.ReferenceLine_DisplayName, data.Name);

            AssertEqualStyle(mapLineData.Style, Color.Red, 3, DashStyle.Solid);
        }

        [Test]
        public void Create_NoReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MapDataFactory.Create((ReferenceLine)null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("referenceLine", parameter);
        }

        [Test]
        public void Create_GivenHydraulicDatabase_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var database = new HydraulicBoundaryDatabase();
            database.Locations.AddRange(pointsOne.Select(p => new HydraulicBoundaryLocation(0, "", p.X, p.Y)));

            // Call
            MapData data = MapDataFactory.Create(database);

            // Assert
            Assert.IsInstanceOf<MapPointData>(data);
            var mapPointData = (MapPointData)data;
            Assert.AreEqual(1, mapPointData.Features.Count());
            Assert.AreEqual(1, mapPointData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, mapPointData.Features.ElementAt(0).MapGeometries.ElementAt(0));

            Assert.AreEqual(Common.Data.Properties.Resources.HydraulicBoundaryConditions_DisplayName, data.Name);

            AssertEqualStyle(mapPointData.Style, Color.DarkBlue, 6, PointSymbol.Circle);
        }

        [Test]
        public void Create_NoHydraulicDatabase_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MapDataFactory.Create((HydraulicBoundaryDatabase)null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryDatabase", parameter);
        }

        private void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }
        
        private void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p.X, p.Y)), geometry.Points);
        } 
    }
}