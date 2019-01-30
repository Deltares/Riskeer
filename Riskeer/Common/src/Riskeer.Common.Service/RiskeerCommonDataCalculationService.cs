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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service.Properties;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Service for providing common data calculation services.
    /// </summary>
    public static class RiskeerCommonDataCalculationService
    {
        private static readonly Range<double> normValidityRange = new Range<double>(0, 1);
        private static readonly Range<double> contributionValidityRange = new Range<double>(0, 100);

        /// <summary>
        /// Determines whether the calculated output is converged.
        /// </summary>
        /// <param name="converged">The value indicating whether convergence has been reached.</param>
        /// <returns><see cref="CalculationConvergence.CalculatedConverged"/> if the calculated output converged,
        /// <see cref="CalculationConvergence.CalculatedNotConverged"/> if the calculated output did not converge,
        /// <see cref="CalculationConvergence.NotCalculated"/> if no convergence was determined.</returns>
        public static CalculationConvergence GetCalculationConvergence(bool? converged)
        {
            return converged.HasValue
                       ? converged.Value
                             ? CalculationConvergence.CalculatedConverged
                             : CalculationConvergence.CalculatedNotConverged
                       : CalculationConvergence.NotCalculated;
        }

        /// <summary>
        /// Gets the required probability which is needed in profile specific calculations.
        /// </summary>
        /// <param name="norm">The assessment section norm.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The profile specific required probability.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="norm"/> is not in the interval [0.0, 1.0] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="failureMechanismContribution"/> is not in the interval [0.0, 100.0] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="n"/> is not larger than 0.</item>
        /// </list>
        /// </exception>
        public static double ProfileSpecificRequiredProbability(double norm, double failureMechanismContribution, RoundedDouble n)
        {
            if (!normValidityRange.InRange(norm))
            {
                string message = string.Format(Resources.RiskeerCommonDataCalculationService_ProfileSpecificRequiredProbability_Norm_must_be_in_Range_0_,
                                               normValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(nameof(norm), norm, message);
            }

            if (!contributionValidityRange.InRange(failureMechanismContribution))
            {
                string message = string.Format(Resources.RiskeerCommonDataCalculationService_ProfileSpecificRequiredProbability_Contribution_must_be_in_Range_0_,
                                               contributionValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(nameof(failureMechanismContribution), failureMechanismContribution,
                                                      message);
            }

            if (n <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), n,
                                                      Resources.RiskeerCommonDataCalculationService_ProfileSpecificRequiredProbability_N_must_be_larger_than_0);
            }

            return norm * (failureMechanismContribution / 100) / n;
        }
    }
}