﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Drawing;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class AssemblyMapDataFactoryTest
    {
        [Test]
        public void CreateAssemblyMapDataCollection_ReturnsEmptyMapDataCollection()
        {
            // Call
            MapDataCollection data = AssemblyMapDataFactory.CreateAssemblyMapDataCollection();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
            Assert.AreEqual("Toetsoordeel", data.Name);
        }

        [Test]
        public void CreateFailureMechanismSectionsMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = AssemblyMapDataFactory.CreateSimpleAssemblyMapData();

            // Assert
            AssertAssemblyMapLineData("Toetsoordeel eenvoudige toets", false, data);
        }

        [Test]
        public void CreateDetailedAssemblyMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = AssemblyMapDataFactory.CreateDetailedAssemblyMapData();

            // Assert
            AssertAssemblyMapLineData("Toetsoordeel gedetailleerde toets", false, data);
        }

        [Test]
        public void CreateTailorMadeAssemblyMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = AssemblyMapDataFactory.CreateTailorMadeAssemblyMapData();

            // Assert
            AssertAssemblyMapLineData("Toetsoordeel toets op maat", false, data);
        }

        [Test]
        public void CreateCombinedAssemblyMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = AssemblyMapDataFactory.CreateCombinedAssemblyMapData();

            // Assert
            AssertAssemblyMapLineData("Gecombineerd toetsoordeel", true, data);
        }

        [Test]
        public void CreateAssemblyMapData_ReturnsEmptyMapLineDataWithExpectedStyling()
        {
            // Call
            MapLineData data = AssemblyMapDataFactory.CreateAssemblyMapData();

            // Assert
            AssertAssemblyMapLineData("Duidingsklasse per vak", true, data);
        }

        private static void AssertAssemblyMapLineData(string expectedName, bool expectedVisibility, MapLineData actualMapLineData)
        {
            CollectionAssert.IsEmpty(actualMapLineData.Features);
            Assert.AreEqual(expectedName, actualMapLineData.Name);
            Assert.AreEqual(expectedVisibility, actualMapLineData.IsVisible);
            Assert.AreEqual("Categorie", actualMapLineData.SelectedMetaDataAttribute);
            RiskeerMapDataFactoryTestHelper.AssertEqualStyle(actualMapLineData.Style, Color.Empty, 6, LineDashStyle.Solid);
            MapThemeTestHelper.AssertDisplayFailureMechanismSectionAssemblyGroupMapTheme(actualMapLineData.Theme);
        }
    }
}