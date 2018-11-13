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
using Core.Components.Gis.Theme;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapTheme"/> instances.
    /// </summary>
    public static class MapThemeFactory
    {
        /// <summary>
        /// Creates a <see cref="MapTheme"/> based on the values of <see cref="DisplayFailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <returns>The created <see cref="MapTheme"/>.</returns>
        public static MapTheme CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme()
        {
            return new MapTheme(Resources.AssemblyCategory_Group_DisplayName, new[]
            {
                new CategoryTheme(Color.FromArgb(255, 0, 255, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv)),
                new CategoryTheme(Color.FromArgb(255, 118, 147, 60), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv)),
                new CategoryTheme(Color.FromArgb(255, 255, 255, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv)),
                new CategoryTheme(Color.FromArgb(255, 204, 192, 218), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv)),
                new CategoryTheme(Color.FromArgb(255, 255, 153, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv)),
                new CategoryTheme(Color.FromArgb(255, 255, 0, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv)),
                new CategoryTheme(Color.FromArgb(255, 255, 255, 255), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv)),
                new CategoryTheme(Color.FromArgb(0, 0, 0, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable)),
                new CategoryTheme(Color.FromArgb(0, 0, 0, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.None))
            });
        }

        private static ValueCriterion CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup category)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(category).DisplayName);
        }
    }
}