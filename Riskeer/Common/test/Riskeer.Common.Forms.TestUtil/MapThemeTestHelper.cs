// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to assert properties of a <see cref="MapTheme{TCategoryTheme}"/>
    /// </summary>
    public static class MapThemeTestHelper
    {
        /// <summary>
        /// Asserts whether the <paramref name="theme"/> is configured for values
        /// of type <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="theme">The <see cref="MapTheme{T}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="theme"/> does not have the expected attribute name it categorizes its data on.</item>
        /// <item><paramref name="theme"/> does not have the expected number of criteria as themes.</item>
        /// <item><paramref name="theme"/> does not have the expected categorical criteria as themes.</item>
        /// </list>
        /// </exception>
        public static void AssertFailureMechanismSectionAssemblyGroupMapTheme(MapTheme<LineCategoryTheme> theme)
        {
            Assert.AreEqual("Duidingsklasse", theme.AttributeName);
            Assert.AreEqual(11, theme.CategoryThemes.Count());
            AssertCategoryTheme("+III", Color.FromArgb(255, 34, 139, 34), theme.CategoryThemes.ElementAt(0));
            AssertCategoryTheme("+II", Color.FromArgb(255, 146, 208, 80), theme.CategoryThemes.ElementAt(1));
            AssertCategoryTheme("+I", Color.FromArgb(255, 198, 224, 180), theme.CategoryThemes.ElementAt(2));
            AssertCategoryTheme("0", Color.FromArgb(255, 255, 255, 0), theme.CategoryThemes.ElementAt(3));
            AssertCategoryTheme("-I", Color.FromArgb(255, 255, 165, 0), theme.CategoryThemes.ElementAt(4));
            AssertCategoryTheme("-II", Color.FromArgb(255, 255, 0, 0), theme.CategoryThemes.ElementAt(5));
            AssertCategoryTheme("-III", Color.FromArgb(255, 178, 34, 34), theme.CategoryThemes.ElementAt(6));
            AssertCategoryTheme("Do", Color.FromArgb(255, 255, 90, 172), theme.CategoryThemes.ElementAt(7));
            AssertCategoryTheme("NDo", Color.FromArgb(255, 192, 192, 192), theme.CategoryThemes.ElementAt(8));
            AssertCategoryTheme("NR", Color.FromArgb(255, 38, 245, 245), theme.CategoryThemes.ElementAt(9));
            AssertCategoryTheme(string.Empty, Color.FromArgb(0, 0, 0, 0), theme.CategoryThemes.ElementAt(10));
        }

        private static void AssertCategoryTheme(string expectedValue, Color expectedColor, LineCategoryTheme categoryTheme)
        {
            Assert.AreEqual(expectedValue, categoryTheme.Criterion.Value);
            Assert.AreEqual(ValueCriterionOperator.EqualValue, categoryTheme.Criterion.ValueOperator);
            AssertLineStyle(expectedColor, categoryTheme.Style);
        }

        private static void AssertLineStyle(Color expectedColor, LineStyle actualLineStyle)
        {
            Assert.AreEqual(expectedColor, actualLineStyle.Color);
            Assert.AreEqual(6, actualLineStyle.Width);
            Assert.AreEqual(LineDashStyle.Solid, actualLineStyle.DashStyle);
        }
    }
}