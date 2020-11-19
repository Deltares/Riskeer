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
using System.Drawing;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Riskeer.Common.Forms.Factories;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RiskeerMapDataFactoryTest
    {
        [Test]
        public void CreateReferenceLineMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateReferenceLineMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Referentielijn", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.FromArgb(0, 128, 255), 3, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateFailureMechanismSectionsMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateFailureMechanismSectionsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.Khaki, 3, LineDashStyle.Dot);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateFailureMechanismSectionsStartPointMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RiskeerMapDataFactory.CreateFailureMechanismSectionsStartPointMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling (startpunten)", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateFailureMechanismSectionsEndPointMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RiskeerMapDataFactory.CreateFailureMechanismSectionsEndPointMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Vakindeling (eindpunten)", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationsMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RiskeerMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Hydraulische belastingen", data.Name);
            Assert.IsTrue(data.ShowLabels);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.DarkBlue, 6, PointSymbol.Circle);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateDikeProfileMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateDikeProfileMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Dijkprofielen", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.SaddleBrown, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateForeshoreProfileMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateForeshoreProfileMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Voorlandprofielen", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.DarkOrange, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateStructuresMapData_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapPointData data = RiskeerMapDataFactory.CreateStructuresMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Kunstwerken", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.DarkSeaGreen, 15, PointSymbol.Square);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateCalculationsMapData_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerMapDataFactory.CreateCalculationsMapData(null, Color.MediumPurple);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateCalculationsMapData_WithoutParameters_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateCalculationsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Berekeningen", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.MediumPurple, 2, LineDashStyle.Dash);
        }

        [Test]
        public void CreateCalculationsMapData_WithParameters_ReturnsEmptyMapPointDataWithExpectedStyling()
        {
            // Setup
            const string calculationsName = "Berekeningen";
            Color color = Color.MediumPurple;

            // Call
            MapLineData data = RiskeerMapDataFactory.CreateCalculationsMapData(calculationsName, color);

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual(calculationsName, data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, color, 2, LineDashStyle.Dash);
        }

        [Test]
        public void CreateSurfaceLinesMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateSurfaceLinesMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Profielschematisaties", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.DarkSeaGreen, 2, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateStochasticSoilModelsMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = RiskeerMapDataFactory.CreateStochasticSoilModelsMapData();

            // Assert
            CollectionAssert.IsEmpty(data.Features);
            Assert.AreEqual("Stochastische ondergrondmodellen", data.Name);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(data.Style, Color.FromArgb(70, Color.SaddleBrown), 5, LineDashStyle.Solid);
            Assert.AreEqual("Naam", data.SelectedMetaDataAttribute);
        }

        [Test]
        public void CreateSectionsMapDataCollection_ReturnsEmptyMapDataCollectionWithExpectedName()
        {
            // Call
            MapDataCollection data = RiskeerMapDataFactory.CreateSectionsMapDataCollection();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Vakindeling", data.Name);
        }
    }
}