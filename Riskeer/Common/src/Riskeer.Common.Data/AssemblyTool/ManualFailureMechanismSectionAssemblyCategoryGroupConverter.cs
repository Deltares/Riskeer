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

using System;
using System.ComponentModel;
using Riskeer.Common.Primitives;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// The converter that converts <see cref="ManualFailureMechanismSectionAssemblyCategoryGroup"/>
    /// to <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public static class ManualFailureMechanismSectionAssemblyCategoryGroupConverter
    {
        /// <summary>
        /// Converts a <see cref="ManualFailureMechanismSectionAssemblyCategoryGroup"/> into a
        /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The <see cref="ManualFailureMechanismSectionAssemblyCategoryGroup"/> to convert.</param>
        /// <returns>The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on <paramref name="categoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryGroup"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryGroup"/> is a valid value, but not supported.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup Convert(ManualFailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            if (!Enum.IsDefined(typeof(ManualFailureMechanismSectionAssemblyCategoryGroup), categoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(categoryGroup),
                                                       (int) categoryGroup,
                                                       typeof(ManualFailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (categoryGroup)
            {
                case ManualFailureMechanismSectionAssemblyCategoryGroup.None:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}