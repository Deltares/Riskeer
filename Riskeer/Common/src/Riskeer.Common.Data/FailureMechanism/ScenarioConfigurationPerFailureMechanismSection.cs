// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
// 
// This file is part of DiKErnel.
// 
// DiKErnel is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with this
// program. If not, see <http://www.gnu.org/licenses/>.
// 
// All names, logos, and references to "Deltares" are registered trademarks of Stichting
// Deltares and remain full property of Stichting Deltares at all times. All rights reserved.

using System;
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.FailureMechanism
{
    
    /// <summary>
    /// This class holds the information of the scenario configuration of the <see cref="FailureMechanismSection"/>.
    /// </summary>
    public class ScenarioConfigurationPerFailureMechanismSection : Observable
    {
        private static readonly Range<double> validityRangeA = new Range<double>(0, 1);
        private double a;

        /// <summary>
        /// Creates a new instance of <see cref="Riskeer.Piping.Data.PipingScenarioConfigurationPerFailureMechanismSection"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the scenario configuration from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public ScenarioConfigurationPerFailureMechanismSection(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Section = section;
        }
        
        /// <summary>
        /// Gets or sets 'a' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not in the range [0, 1].</exception>
        public double A
        {
            get => a;
            set
            {
                if (!validityRangeA.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          string.Format(Resources.ProbabilityAssessmentInput_A_Value_must_be_in_Range_0_,
                                                                        validityRangeA.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture)));
                }

                a = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; }
    }
}