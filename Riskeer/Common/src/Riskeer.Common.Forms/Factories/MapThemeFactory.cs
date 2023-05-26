// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Util.Enums;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Riskeer.AssemblyTool.Data;
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
        public static MapTheme<LineCategoryTheme> CreateFailureMechanismSectionAssemblyGroupMapTheme()
        {
            return new MapTheme<LineCategoryTheme>(Resources.AssemblyGroup_DisplayName, new[]
            {
                CreateAssemblyGroupTheme(Color.FromArgb(255, 34, 139, 34), FailureMechanismSectionAssemblyGroup.III),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 146, 208, 80), FailureMechanismSectionAssemblyGroup.II),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 198, 224, 180), FailureMechanismSectionAssemblyGroup.I),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 255, 0), FailureMechanismSectionAssemblyGroup.Zero),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 165, 0), FailureMechanismSectionAssemblyGroup.IMin),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 0, 0), FailureMechanismSectionAssemblyGroup.IIMin),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 178, 34, 34), FailureMechanismSectionAssemblyGroup.IIIMin),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 255, 90, 172), FailureMechanismSectionAssemblyGroup.Dominant),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 192, 192, 192), FailureMechanismSectionAssemblyGroup.NotDominant),
                CreateAssemblyGroupTheme(Color.FromArgb(255, 38, 245, 245), FailureMechanismSectionAssemblyGroup.NotRelevant),
                CreateAssemblyGroupTheme(Color.FromArgb(0, 0, 0, 0), FailureMechanismSectionAssemblyGroup.NoResult)
            });
        }

        private static LineCategoryTheme CreateAssemblyGroupTheme(Color color, FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            var lineStyle = new LineStyle
            {
                Color = color,
                DashStyle = lineDashStyle,
                Width = lineWidth
            };

            return new LineCategoryTheme(CreateCriterion(assemblyGroup), lineStyle);
        }

        private static ValueCriterion CreateCriterion(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      EnumDisplayNameHelper.GetDisplayName(assemblyGroup));
        }
    }
}