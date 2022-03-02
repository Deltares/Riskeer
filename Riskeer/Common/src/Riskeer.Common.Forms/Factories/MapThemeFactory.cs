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
        public static MapTheme<LineCategoryTheme> CreateDisplayFailureMechanismSectionAssemblyGroupMapTheme()
        {
            return new MapTheme<LineCategoryTheme>(Resources.AssemblyGroup_DisplayName, new[]
            {
                CreateAssemblyGroupTheme(Color.FromArgb(255, 34, 139, 34), DisplayFailureMechanismSectionAssemblyGroup.III),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 146, 208, 80), DisplayFailureMechanismSectionAssemblyGroup.II),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 198, 224, 180), DisplayFailureMechanismSectionAssemblyGroup.I),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 255, 0), DisplayFailureMechanismSectionAssemblyGroup.Zero),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 165, 0), DisplayFailureMechanismSectionAssemblyGroup.IMin),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 0, 0), DisplayFailureMechanismSectionAssemblyGroup.IIMin),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 178, 34, 34), DisplayFailureMechanismSectionAssemblyGroup.IIIMin),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 90, 172), DisplayFailureMechanismSectionAssemblyGroup.Dominant),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 192, 192, 192), DisplayFailureMechanismSectionAssemblyGroup.NotDominant),
                CreateAssemblyGroupTheme(Color.FromArgb(0, 0, 0, 0), DisplayFailureMechanismSectionAssemblyGroup.GR)
            });
        }

        private static LineCategoryTheme CreateAssemblyGroupTheme(Color color, DisplayFailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            var lineStyle = new LineStyle
            {
                Color = color,
                DashStyle = lineDashStyle,
                Width = lineWidth
            };

            return new LineCategoryTheme(CreateCriterion(assemblyGroup), lineStyle);
        }

        private static ValueCriterion CreateCriterion(DisplayFailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyGroup>(assemblyGroup).DisplayName);
        }
    }
}