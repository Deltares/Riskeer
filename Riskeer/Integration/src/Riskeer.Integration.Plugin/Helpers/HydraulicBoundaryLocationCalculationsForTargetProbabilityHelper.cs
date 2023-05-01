// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Integration.Plugin.Helpers
{
    /// <summary>
    /// Helper class for <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationsForTargetProbabilityHelper
    {
        /// <summary>
        /// Creates a new <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>
        /// and sets the calculations for all locations of the <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to get the locations from.</param>
        /// <param name="probability">The probability of the calculations to add.</param>
        /// <returns>The created <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public static HydraulicBoundaryLocationCalculationsForTargetProbability Create(IAssessmentSection assessmentSection, double probability = 0.01)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(probability);

            calculationsForTargetProbability.HydraulicBoundaryLocationCalculations.AddRange(
                assessmentSection.HydraulicBoundaryData.GetLocations().Select(hbl => new HydraulicBoundaryLocationCalculation(hbl)));

            return calculationsForTargetProbability;
        }
    }
}