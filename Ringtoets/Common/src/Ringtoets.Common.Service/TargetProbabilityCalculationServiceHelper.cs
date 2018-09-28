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
using Core.Common.Util;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// This class defines helper methods for performing target probability calculations.
    /// </summary>
    public static class TargetProbabilityCalculationServiceHelper
    {
        /// <summary>
        /// Validates the provided target probability. Log messages are handled during the execution of the operation.
        /// </summary>
        /// <param name="targetProbability">The target probability to validate.</param>
        /// <param name="handleLogMessageAction">The action to perform for handling log messages.</param>
        /// <returns><c>true</c> if <paramref name="targetProbability"/> gives no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="handleLogMessageAction"/> is <c>null</c>.</exception>
        public static bool ValidateTargetProbability(double targetProbability, Action<string> handleLogMessageAction)
        {
            if (handleLogMessageAction == null)
            {
                throw new ArgumentNullException(nameof(handleLogMessageAction));
            }

            if (double.IsNaN(targetProbability))
            {
                handleLogMessageAction(Resources.TargetProbabilityCalculationServiceHelper_ValidateTargetProbability_Target_probability_invalid);

                return false;
            }

            double targetBeta = StatisticsConverter.ProbabilityToReliability(targetProbability);
            if (targetBeta < -1)
            {
                handleLogMessageAction(Resources.TargetProbabilityCalculationServiceHelper_ValidateTargetProbability_Target_probability_too_big);
                return false;
            }

            if (targetBeta > 40)
            {
                handleLogMessageAction(Resources.TargetProbabilityCalculationServiceHelper_ValidateTargetProbability_Target_probability_too_small);
                return false;
            }

            return true;
        }
    }
}