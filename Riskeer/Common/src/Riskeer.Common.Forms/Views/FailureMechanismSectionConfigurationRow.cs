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
using Core.Common.Base.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="FailureMechanismSectionConfiguration"/>.
    /// </summary>
    public class FailureMechanismSectionConfigurationRow : FailureMechanismSectionRow
    {
        protected readonly FailureMechanismSectionConfiguration SectionConfiguration;
        private readonly double b;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionConfigurationRow"/>.
        /// </summary>
        /// <param name="sectionConfiguration">The  section configuration to use.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfiguration"/> is <c>null</c>.</exception>
        public FailureMechanismSectionConfigurationRow(FailureMechanismSectionConfiguration sectionConfiguration,
                                                       double sectionStart, double sectionEnd, double b)
            : base(sectionConfiguration?.Section ?? throw new ArgumentNullException(nameof(sectionConfiguration)), sectionStart, sectionEnd)
        {
            SectionConfiguration = sectionConfiguration;
            this.b = b;
        }

        /// <summary>
        /// Gets or sets the 'a' parameter.
        /// [-]
        /// </summary>
        public virtual RoundedDouble A
        {
            get => SectionConfiguration.A;
            set
            {
                if (!SectionConfiguration.A.Equals(value))
                {
                    SectionConfiguration.A = value;
                    SectionConfiguration.NotifyObservers();
                }
            }
        }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// [-]
        /// </summary>
        public RoundedDouble N => new RoundedDouble(2, SectionConfiguration.GetN(b));
    }
}