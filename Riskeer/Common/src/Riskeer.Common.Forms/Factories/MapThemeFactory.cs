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

using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Util;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;
using Riskeer.Common.Forms.Helpers;
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
        /// Creates a <see cref="MapTheme{T}"/> based on the values of <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <returns>The created <see cref="MapTheme{T}"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static MapTheme<LineCategoryTheme> CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme()
        {
            var categoryThemes = new List<LineCategoryTheme>();

            foreach (FailureMechanismSectionAssemblyGroup enumValue in Enum.GetValues(typeof(FailureMechanismSectionAssemblyGroup)))
            {
                LineCategoryTheme theme = CreateCategoryTheme(AssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(enumValue),
                                                              DisplayFailureMechanismSectionAssemblyGroupConverter.Convert(enumValue));

                categoryThemes.Add(theme);
            }

            return new MapTheme<LineCategoryTheme>(Resources.AssemblyGroup_DisplayName, categoryThemes);
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