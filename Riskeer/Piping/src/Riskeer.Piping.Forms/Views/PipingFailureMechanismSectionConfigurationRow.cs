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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingFailureMechanismSectionConfiguration"/>
    /// </summary>
    public class PipingFailureMechanismSectionConfigurationRow : FailureMechanismSectionConfigurationRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionConfiguration"/>.
        /// </summary>
        /// <param name="sectionConfiguration">The  section configuration to use.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfiguration"/> is <c>null</c>.</exception>
        public PipingFailureMechanismSectionConfigurationRow(PipingFailureMechanismSectionConfiguration sectionConfiguration, double sectionStart, double sectionEnd, double b) 
            : base(sectionConfiguration, sectionStart, sectionEnd, b) {}
        
        /// <summary>
        /// Gets the failure mechanism sensitive section length.
        /// [m]
        /// </summary>
        public RoundedDouble FailureMechanismSensitiveSectionLength => new RoundedDouble(2, SectionConfiguration.GetFailureMechanismSensitiveSectionLength());
    }
}