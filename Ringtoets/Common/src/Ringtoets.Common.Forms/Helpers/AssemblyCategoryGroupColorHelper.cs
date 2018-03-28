﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.ComponentModel;
using System.Drawing;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for determining the colors belonging to various assembly category groups.
    /// </summary>
    public static class AssemblyCategoryGroupColorHelper
    {
        /// <summary>
        /// Gets the color for an assessment section assembly category group.
        /// </summary>
        /// <param name="assemblyCategoryGroup">The category group to get the color for.</param>
        /// <returns>The <see cref="Color"/> corresponding to the given category group.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyCategoryGroup"/>
        /// has an invalid value for <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyCategoryGroup"/>
        /// is not supported.</exception>
        public static Color GetAssessmentSectionAssemblyCategoryGroupColor(AssessmentSectionAssemblyCategoryGroup assemblyCategoryGroup)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyCategoryGroup), assemblyCategoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyCategoryGroup),
                                                       (int) assemblyCategoryGroup,
                                                       typeof(AssessmentSectionAssemblyCategoryGroup));
            }

            switch (assemblyCategoryGroup)
            {
                case AssessmentSectionAssemblyCategoryGroup.APlus:
                    return Color.FromArgb(0, 255, 0);
                case AssessmentSectionAssemblyCategoryGroup.A:
                    return Color.FromArgb(118, 147, 60);
                case AssessmentSectionAssemblyCategoryGroup.B:
                    return Color.FromArgb(255, 255, 0);
                case AssessmentSectionAssemblyCategoryGroup.C:
                    return Color.FromArgb(255, 153, 0);
                case AssessmentSectionAssemblyCategoryGroup.D:
                    return Color.FromArgb(255, 0, 0);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the color for a failure mechanism assembly category group.
        /// </summary>
        /// <param name="assemblyCategoryGroup">The category group to get the color for.</param>
        /// <returns>The <see cref="Color"/> corresponding to the given category group.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyCategoryGroup"/>
        /// has an invalid value for <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyCategoryGroup"/>
        /// is not supported.</exception>
        public static Color GetFailureMechanismSectionAssemblyCategoryGroupColor(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), assemblyCategoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyCategoryGroup),
                                                       (int) assemblyCategoryGroup,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (assemblyCategoryGroup)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return Color.FromArgb(0, 255, 0);
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return Color.FromArgb(118, 147, 60);
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return Color.FromArgb(255, 255, 0);
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return Color.FromArgb(204, 192, 218);
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return Color.FromArgb(255, 153, 0);
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return Color.FromArgb(255, 0, 0);
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return Color.FromArgb(255, 255, 255);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}