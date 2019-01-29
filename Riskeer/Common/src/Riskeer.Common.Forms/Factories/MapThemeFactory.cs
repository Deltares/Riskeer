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
using Core.Common.Util;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Ringtoets.Common.Forms.Properties;
using Riskeer.AssemblyTool.Forms;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapTheme{T}"/> instances.
    /// </summary>
    public static class MapThemeFactory
    {
        private const int lineWidth = 6;
        private const LineDashStyle lineDashStyle = LineDashStyle.Solid;

        /// <summary>
        /// Creates a <see cref="MapTheme{T}"/> based on the values of <see cref="DisplayFailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <returns>The created <see cref="MapTheme{T}"/>.</returns>
        public static MapTheme<LineCategoryTheme> CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme()
        {
            return new MapTheme<LineCategoryTheme>(Resources.AssemblyCategory_Group_DisplayName, new[]
            {
                CreateCategoryTheme(Color.FromArgb(255, 0, 255, 0), DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv),
                CreateCategoryTheme(Color.FromArgb(255, 118, 147, 60), DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 255, 0), DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv),
                CreateCategoryTheme(Color.FromArgb(255, 204, 192, 218), DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 153, 0), DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 0, 0), DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 255, 255), DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv),
                CreateCategoryTheme(Color.FromArgb(0, 0, 0, 0), DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable),
                CreateCategoryTheme(Color.FromArgb(0, 0, 0, 0), DisplayFailureMechanismSectionAssemblyCategoryGroup.None)
            });
        }

        private static LineCategoryTheme CreateCategoryTheme(Color color, DisplayFailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            var lineStyle = new LineStyle
            {
                Color = color,
                DashStyle = lineDashStyle,
                Width = lineWidth
            };

            return new LineCategoryTheme(CreateCriterion(categoryGroup), lineStyle);
        }

        private static ValueCriterion CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup category)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(category).DisplayName);
        }
    }
}