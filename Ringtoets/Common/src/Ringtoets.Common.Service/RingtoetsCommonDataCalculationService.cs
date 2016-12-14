// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for providing common data calculation services.
    /// </summary>
    public static class RingtoetsCommonDataCalculationService
    {
        /// <summary>
        /// Determines whether the calculated output is converged,
        /// based on the <paramref name="reliabilityIndex"/> and the <paramref name="norm"/>.
        /// </summary>
        /// <param name="reliabilityIndex">The resultant reliability index after a calculation.</param>
        /// <param name="norm">The norm used during the calculation.</param>
        /// <returns><c>True</c> if the solution converged, <c>false</c> if otherwise.</returns>
        public static CalculationConvergence CalculationConverged(double reliabilityIndex, double norm)
        {
            return Math.Abs(reliabilityIndex - StatisticsConverter.ProbabilityToReliability(norm)) <= 1.0e-3 ?
                       CalculationConvergence.CalculatedConverged :
                       CalculationConvergence.CalculatedNotConverged;
        }

        /// <summary>
        /// Gets the required probability which is needed in profile specific calculations.
        /// </summary>
        /// <param name="norm">The assessment section norm.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The profile specific required probability.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="failureMechanismContribution"/> 
        /// or <paramref name="n"/> is not greater than 0.</exception>
        public static double ProfileSpecificRequiredProbability(double norm, double failureMechanismContribution, int n)
        {
            if (!(failureMechanismContribution > 0))
            {
                throw new ArgumentOutOfRangeException("failureMechanismContribution", failureMechanismContribution,
                                                      Resources.RingtoetsCommonDataCalculationService_ProfileSpecificRequiredProbability_Contribution_is_zero);
            }
            if (!(n > 0))
            {
                throw new ArgumentOutOfRangeException("n", n,
                                                      Resources.RingtoetsCommonDataCalculationService_ProfileSpecificRequiredProbability_N_is_zero);
            }

            return norm*(failureMechanismContribution/100)/n;
        }
    }
}