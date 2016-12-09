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

using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapDataFactoryTest
    {
        [Test]
        public void CreateReferenceLineMapData_ReturnsEmptyMapLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateReferenceLineMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Referentielijn", data.Name);
            AssertEqualStyle(data.Style, Color.Red, 3, DashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateFailureMechanismSectionsMapData_ReturnsEmptyChartLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling", data.Name);
            AssertEqualStyle(data.Style, Color.Khaki, 3, DashStyle.Dot);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateFailureMechanismSectionsStartPointMapData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling (startpunten)", data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateFailureMechanismSectionsEndPointMapData_ReturnsEmptyChartPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling (eindpunten)", data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationsMapData_ReturnsEmptyMapPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Hydraulische randvoorwaarden", data.Name);
            Assert.IsTrue(data.ShowLabels);
            AssertEqualStyle(data.Style, Color.DarkBlue, 6, PointSymbol.Circle);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateDikeProfileMapData_ReturnsEmptyMapLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateDikeProfileMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Dijkprofielen", data.Name);
            AssertEqualStyle(data.Style, Color.SaddleBrown, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateForeshoreProfileMapData_ReturnsEmptyMapLineDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Voorlandprofielen", data.Name);
            AssertEqualStyle(data.Style, Color.DarkOrange, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateStructuresMapData_ReturnsEmptyMapPointDataWithDefaultStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateStructuresMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Kunstwerken", data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 15, PointSymbol.Square);
        }

        [Test]
        public void CreateCalculationsMapData_ReturnsEmptyMapPointDataWithDefaultStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateCalculationsMapData();

            // Assert
            Assert.IsEmpty(data.Features);
            Assert.AreEqual("Berekeningen", data.Name);
            AssertEqualStyle(data.Style, Color.MediumPurple, 2, DashStyle.Dash);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }

        private static void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }
    }
}