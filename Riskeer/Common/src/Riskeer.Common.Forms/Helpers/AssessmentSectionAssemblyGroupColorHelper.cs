// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for determining the colors belonging to various assessment section assembly groups.
    /// </summary>
    public static class AssessmentSectionAssemblyGroupColorHelper
    {
        /// <summary>
        /// Gets the color for an assessment section assembly group.
        /// </summary>
        /// <param name="assessmentSectionAssemblyGroup">The assembly group to get the color for.</param>
        /// <returns>The <see cref="Color"/> corresponding to the given assembly group.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSectionAssemblyGroup"/>
        /// has an invalid value for <see cref="AssessmentSectionAssemblyGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSectionAssemblyGroup"/>
        /// is not supported.</exception>
        public static Color GetAssessmentSectionAssemblyGroupColor(AssessmentSectionAssemblyGroup assessmentSectionAssemblyGroup)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyGroup), assessmentSectionAssemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assessmentSectionAssemblyGroup),
                                                       (int) assessmentSectionAssemblyGroup,
                                                       typeof(AssessmentSectionAssemblyGroup));
            }

            switch (assessmentSectionAssemblyGroup)
            {
                case AssessmentSectionAssemblyGroup.APlus:
                    return Color.FromArgb(0, 255, 0);
                case AssessmentSectionAssemblyGroup.A:
                    return Color.FromArgb(118, 147, 60);
                case AssessmentSectionAssemblyGroup.B:
                    return Color.FromArgb(255, 255, 0);
                case AssessmentSectionAssemblyGroup.C:
                    return Color.FromArgb(255, 153, 0);
                case AssessmentSectionAssemblyGroup.D:
                    return Color.FromArgb(255, 0, 0);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}