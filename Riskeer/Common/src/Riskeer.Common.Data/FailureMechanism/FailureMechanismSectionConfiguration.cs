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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class holds the configuration of a <see cref="FailureMechanismSection"/>.
    /// </summary>
    public class FailureMechanismSectionConfiguration : Observable
    {
        private const int aNrOfDecimals = 3;

        private static readonly Range<RoundedDouble> validityRangeA = new Range<RoundedDouble>(
            new RoundedDouble(aNrOfDecimals), new RoundedDouble(aNrOfDecimals, 1));

        private RoundedDouble a;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionConfiguration"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> the configuration belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public FailureMechanismSectionConfiguration(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            A = new RoundedDouble(aNrOfDecimals, 1);
            Section = section;
        }

        /// <summary>
        /// Gets or sets the 'a' parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not in the range [0, 1].</exception>
        public RoundedDouble A
        {
            get => a;
            set
            {
                RoundedDouble newA = value.ToPrecision(aNrOfDecimals);
                if (!validityRangeA.InRange(newA))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          string.Format(Resources.FailureMechanismSectionConfiguration_A_Value_must_be_in_Range_0_,
                                                                        validityRangeA.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture)));
                }

                a = newA;
            }
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; }
    }
}