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
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using NUnit.Framework;
using Ringtoets.Common.Forms.Factories;

namespace Ringtoets.Common.Forms.Test.Factories
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

        private static void AssertAssemblyMapLineData(string expectedName, bool expectedVisibility, MapLineData actualMapLineData)
        {
            CollectionAssert.IsEmpty(actualMapLineData.Features);
            Assert.AreEqual(expectedName, actualMapLineData.Name);
            Assert.AreEqual(expectedVisibility, actualMapLineData.IsVisible);
            Assert.AreEqual("Categorie", actualMapLineData.SelectedMetaDataAttribute);
            AssertEqualStyle(actualMapLineData.Style, Color.Empty, 2, LineDashStyle.Solid);
            AssertMapTheme(actualMapLineData.MapTheme);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, LineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }

        private static void AssertMapTheme(MapTheme theme)
        {
            Assert.AreEqual("Categorie", theme.AttributeName);
            Assert.AreEqual(9, theme.CategoryThemes.Count());
            AssertCategoryTheme("Iv", Color.FromArgb(255, 0, 255, 0), theme.CategoryThemes.First());
            AssertCategoryTheme("IIv", Color.FromArgb(255, 118, 147, 60), theme.CategoryThemes.ElementAt(1));
            AssertCategoryTheme("IIIv", Color.FromArgb(255, 255, 255, 0), theme.CategoryThemes.ElementAt(2));
            AssertCategoryTheme("IVv", Color.FromArgb(255, 204, 192, 218), theme.CategoryThemes.ElementAt(3));
            AssertCategoryTheme("Vv", Color.FromArgb(255, 255, 153, 0), theme.CategoryThemes.ElementAt(4));
            AssertCategoryTheme("VIv", Color.FromArgb(255, 255, 0, 0), theme.CategoryThemes.ElementAt(5));
            AssertCategoryTheme("VIIv", Color.FromArgb(255, 255, 255, 255), theme.CategoryThemes.ElementAt(6));
            AssertCategoryTheme("-", Color.HotPink, theme.CategoryThemes.ElementAt(7));
            AssertCategoryTheme(string.Empty, Color.FromArgb(0, 0, 0, 0), theme.CategoryThemes.ElementAt(8));
        }

        private static void AssertCategoryTheme(string expectedValue, Color expectedColor, CategoryTheme categoryTheme)
        {
            Assert.AreEqual(expectedColor, categoryTheme.Color);
            Assert.AreEqual(expectedValue, categoryTheme.Criterion.Value);
            Assert.AreEqual(ValueCriterionOperator.EqualValue, categoryTheme.Criterion.ValueOperator);
        }
    }
}