// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.Forms
{
    /// <summary>
    /// Converter to convert <see cref="AssessmentSectionAssemblyCategoryGroup"/>
    /// into <see cref="DisplayAssessmentSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public static class DisplayAssessmentSectionAssemblyCategoryGroupConverter
    {
        /// <summary>
        /// Converts <see cref="AssessmentSectionAssemblyCategoryGroup"/> into
        /// <see cref="DisplayAssessmentSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The group to convert.</param>
        /// <returns>The converted <see cref="DisplayAssessmentSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="AssessmentSectionAssemblyCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="AssessmentSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static DisplayAssessmentSectionAssemblyCategoryGroup Convert(AssessmentSectionAssemblyCategoryGroup categoryGroup)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyCategoryGroup), categoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(categoryGroup),
                                                       (int) categoryGroup,
                                                       typeof(AssessmentSectionAssemblyCategoryGroup));
            }

            switch (categoryGroup)
            {
                case AssessmentSectionAssemblyCategoryGroup.APlus:
                    return DisplayAssessmentSectionAssemblyCategoryGroup.APlus;
                case AssessmentSectionAssemblyCategoryGroup.A:
                    return DisplayAssessmentSectionAssemblyCategoryGroup.A;
                case AssessmentSectionAssemblyCategoryGroup.B:
                    return DisplayAssessmentSectionAssemblyCategoryGroup.B;
                case AssessmentSectionAssemblyCategoryGroup.C:
                    return DisplayAssessmentSectionAssemblyCategoryGroup.C;
                case AssessmentSectionAssemblyCategoryGroup.D:
                    return DisplayAssessmentSectionAssemblyCategoryGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}