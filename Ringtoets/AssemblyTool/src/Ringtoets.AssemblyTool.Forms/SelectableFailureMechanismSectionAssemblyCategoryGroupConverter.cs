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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.Forms
{
    /// <summary>
    /// Converter to convert into and from <see cref="SelectableFailureMechanismSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public static class SelectableFailureMechanismSectionAssemblyCategoryGroupConverter
    {
        /// <summary>
        /// Converts <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> into
        /// <see cref="SelectableFailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The group to convert.</param>
        /// <returns>The converted <see cref="SelectableFailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static SelectableFailureMechanismSectionAssemblyCategoryGroup ConvertTo(FailureMechanismSectionAssemblyCategoryGroup categoryGroup)
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
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.None;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.Iv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return SelectableFailureMechanismSectionAssemblyCategoryGroup.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts <see cref="SelectableFailureMechanismSectionAssemblyCategoryGroup"/> into
        /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="categoryGroup">The group to convert.</param>
        /// <returns>The converted <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SelectableFailureMechanismSectionAssemblyCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SelectableFailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup ConvertFrom(SelectableFailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            if (!Enum.IsDefined(typeof(SelectableFailureMechanismSectionAssemblyCategoryGroup), categoryGroup))
            {
                throw new InvalidEnumArgumentException(nameof(categoryGroup),
                                                       (int) categoryGroup,
                                                       typeof(SelectableFailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (categoryGroup)
            {
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.None:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case SelectableFailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}