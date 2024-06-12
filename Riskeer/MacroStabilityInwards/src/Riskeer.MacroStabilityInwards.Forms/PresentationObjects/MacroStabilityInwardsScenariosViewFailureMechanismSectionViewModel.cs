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

using System;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.MacroStabilityInwards.Forms.Views;

namespace Riskeer.MacroStabilityInwards.Forms.PresentationObjects
{
    /// <summary>
    /// ViewModel for a <see cref="FailureMechanismSectionConfiguration"/>
    /// as shown in the <see cref="MacroStabilityInwardsScenariosView"/>.
    /// </summary>
    public class MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel"/>.
        /// </summary>
        /// <param name="sectionConfiguration">The wrapped section configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfiguration"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel(
            FailureMechanismSectionConfiguration sectionConfiguration)
        {
            if (sectionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sectionConfiguration));
            }

            SectionConfiguration = sectionConfiguration;
        }

        /// <summary>
        /// Gets the wrapped <see cref="FailureMechanismSectionConfiguration"/>.
        /// </summary>
        public FailureMechanismSectionConfiguration SectionConfiguration { get; }

        public override string ToString()
        {
            return SectionConfiguration.Section.Name;
        }
    }
}