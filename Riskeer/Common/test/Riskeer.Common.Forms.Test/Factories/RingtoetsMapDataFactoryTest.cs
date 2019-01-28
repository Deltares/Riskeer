// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.Common.Forms.Factories;

namespace Ringtoets.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RingtoetsMapDataFactoryTest
    {
        [Test]
        public void CreateReferenceLineMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateReferenceLineMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Referentielijn", data.Name);
            AssertEqualStyle(data.Style, Color.FromArgb(0, 128, 255), 3, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateFailureMechanismSectionsMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling", data.Name);
            AssertEqualStyle(data.Style, Color.Khaki, 3, LineDashStyle.Dot);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateFailureMechanismSectionsStartPointMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling (startpunten)", data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateFailureMechanismSectionsEndPointMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling (eindpunten)", data.Name);
            AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationsMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Hydraulische belastingen", data.Name);
            Assert.IsTrue(data.ShowLabels);
            AssertEqualStyle(data.Style, Color.DarkBlue, 6, PointSymbol.Circle);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateDikeProfileMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateDikeProfileMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Dijkprofielen", data.Name);
            AssertEqualStyle(data.Style, Color.SaddleBrown, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateForeshoreProfileMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateForeshoreProfileMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Voorlandprofielen", data.Name);
            AssertEqualStyle(data.Style, Color.DarkOrange, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateStructuresMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RingtoetsMapDataFactory.CreateStructuresMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Kunstwerken", data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 15, PointSymbol.Square);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateCalculationsMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateCalculationsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Berekeningen", data.Name);
            AssertEqualStyle(data.Style, Color.MediumPurple, 2, LineDashStyle.Dash);
        }

        [Test]
        public void CreateSurfaceLinesMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateSurfaceLinesMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Profielschematisaties", data.Name);
            AssertEqualStyle(data.Style, Color.DarkSeaGreen, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateStochasticSoilModelsMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RingtoetsMapDataFactory.CreateStochasticSoilModelsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Stochastische ondergrondmodellen", data.Name);
            AssertEqualStyle(data.Style, Color.FromArgb(70, Color.SaddleBrown), 5, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateSectionsMapDataCollection_ReturnsEmptyMapDataCollectionWithExpectedName()
        {
            // Call
            MapDataCollection data = RingtoetsMapDataFactory.CreateSectionsMapDataCollection();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Vakindeling", data.Name);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, LineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }

        private static void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }
    }
}