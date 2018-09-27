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
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for displaying <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public static class FailureMechanismSectionAssemblyCategoryGroupHelper
    {
        /// <summary>
        /// Gets the display name of the given <paramref name="assemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="assemblyCategoryGroup">The assembly category group to get the display name for.</param>
        /// <returns>The display name.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static string GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), assemblyCategoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyCategoryGroup),
                                                       (int) assemblyCategoryGroup,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            DisplayFailureMechanismSectionAssemblyCategoryGroup displayCategoryGroup = DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(
                assemblyCategoryGroup);

            return new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(displayCategoryGroup).DisplayName;
        }
    }
}