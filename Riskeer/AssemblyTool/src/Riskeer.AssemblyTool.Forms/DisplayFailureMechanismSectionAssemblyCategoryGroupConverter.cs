// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.Forms
{
    /// <summary>
    /// Converter to convert <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
    /// into <see cref="DisplayFailureMechanismSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public static class DisplayFailureMechanismSectionAssemblyCategoryGroupConverter
    {
        /// <summary>
        /// Converts <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> into
        /// <see cref="DisplayFailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The group to convert.</param>
        /// <returns>The converted <see cref="DisplayFailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static DisplayFailureMechanismSectionAssemblyCategoryGroup Convert(FailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), categoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(categoryGroup),
                                                       (int) categoryGroup,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (categoryGroup)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.None;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}