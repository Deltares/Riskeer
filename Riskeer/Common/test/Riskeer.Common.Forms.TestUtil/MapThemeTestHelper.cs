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
using System.Linq;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using NUnit.Framework;
using Riskeer.AssemblyTool.Forms;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to assert properties of a <see cref="MapTheme{TCategoryTheme}"/>
    /// </summary>
    public static class MapThemeTestHelper
    {
        /// <summary>
        /// Asserts whether the <paramref name="theme"/> is configured for category values
        /// of type <see cref="DisplayFailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="theme">The <see cref="MapTheme{T}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="theme"/> does not have the expected attribute name it categorizes its data on.</item>
        /// <item><paramref name="theme"/> does not have the expected number of criteria as themes.</item>
        /// <item><paramref name="theme"/> does not have the expected categorical criteria as themes.</item>
        /// </list></exception>
        public static void AssertDisplayFailureMechanismSectionAssemblyCategoryGroupMapTheme(MapTheme<LineCategoryTheme> theme)
        {
            Assert.AreEqual("Categorie", theme.AttributeName);
            Assert.AreEqual(9, theme.CategoryThemes.Count());
            AssertCategoryTheme("Iv", Color.FromArgb(255, 0, 255, 0), theme.CategoryThemes.ElementAt(0));
            AssertCategoryTheme("IIv", Color.FromArgb(255, 118, 147, 60), theme.CategoryThemes.ElementAt(1));
            AssertCategoryTheme("IIIv", Color.FromArgb(255, 255, 255, 0), theme.CategoryThemes.ElementAt(2));
            AssertCategoryTheme("IVv", Color.FromArgb(255, 204, 192, 218), theme.CategoryThemes.ElementAt(3));
            AssertCategoryTheme("Vv", Color.FromArgb(255, 255, 153, 0), theme.CategoryThemes.ElementAt(4));
            AssertCategoryTheme("VIv", Color.FromArgb(255, 255, 0, 0), theme.CategoryThemes.ElementAt(5));
            AssertCategoryTheme("VIIv", Color.FromArgb(255, 255, 255, 255), theme.CategoryThemes.ElementAt(6));
            AssertCategoryTheme("-", Color.FromArgb(0, 0, 0, 0), theme.CategoryThemes.ElementAt(7));
            AssertCategoryTheme(string.Empty, Color.FromArgb(0, 0, 0, 0), theme.CategoryThemes.ElementAt(8));
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