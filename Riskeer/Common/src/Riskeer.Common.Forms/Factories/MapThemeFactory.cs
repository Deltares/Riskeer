// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Riskeer.AssemblyTool.Forms;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapTheme{TCategoryTheme}"/> instances.
    /// </summary>
    public static class MapThemeFactory
    {
        private const int lineWidth = 6;
        private const LineDashStyle lineDashStyle = LineDashStyle.Solid;

        /// <summary>
        /// Creates a <see cref="MapTheme{T}"/> based on the values of <see cref="DisplayFailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <returns>The created <see cref="MapTheme{T}"/>.</returns>
        public static MapTheme<LineCategoryTheme> CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme()
        {
            return new MapTheme<LineCategoryTheme>(Resources.AssemblyGroup_Name_DisplayName, new[]
            {
                CreateCategoryTheme(Color.FromArgb(255, 34, 139, 34), DisplayFailureMechanismSectionAssemblyGroup.III),
                CreateCategoryTheme(Color.FromArgb(255, 146, 208, 80), DisplayFailureMechanismSectionAssemblyGroup.II),
                CreateCategoryTheme(Color.FromArgb(255, 198, 224, 180), DisplayFailureMechanismSectionAssemblyGroup.I),
                CreateCategoryTheme(Color.FromArgb(255, 255, 255, 0), DisplayFailureMechanismSectionAssemblyGroup.Zero),
                CreateCategoryTheme(Color.FromArgb(255, 255, 165, 0), DisplayFailureMechanismSectionAssemblyGroup.IMin),
                CreateCategoryTheme(Color.FromArgb(255, 255, 0, 0), DisplayFailureMechanismSectionAssemblyGroup.IIMin),
                CreateCategoryTheme(Color.FromArgb(255, 178, 34, 34), DisplayFailureMechanismSectionAssemblyGroup.IIIMin),
                CreateCategoryTheme(Color.FromArgb(255, 255, 90, 172), DisplayFailureMechanismSectionAssemblyGroup.Dominant),
                CreateCategoryTheme(Color.FromArgb(255, 192, 192, 192), DisplayFailureMechanismSectionAssemblyGroup.NotDominant),
                CreateCategoryTheme(Color.FromArgb(0, 0, 0, 0), DisplayFailureMechanismSectionAssemblyGroup.GR)
            });
        }

        private static LineCategoryTheme CreateCategoryTheme(Color color, DisplayFailureMechanismSectionAssemblyGroup categoryGroup)
        {
            var lineStyle = new LineStyle
            {
                Color = color,
                DashStyle = lineDashStyle,
                Width = lineWidth
            };

            return new LineCategoryTheme(CreateCriterion(categoryGroup), lineStyle);
        }

        private static ValueCriterion CreateCriterion(DisplayFailureMechanismSectionAssemblyGroup category)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyGroup>(category).DisplayName);
        }
    }
}