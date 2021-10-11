// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Forms.PresentationObjects
{
    /// <summary>
    /// Class that represents a selectable target probability for a collection of <see cref="HydraulicBoundaryLocationCalculation"/>.
    /// </summary>
    public class SelectableTargetProbability
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="SelectableTargetProbability"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/>.</param>
        /// <param name="hydraulicBoundaryLocationCalculations">The collection of <see cref="HydraulicBoundaryLocationCalculation"/>.</param>
        /// <param name="waterLevelType">The <see cref="WaveConditionsInputWaterLevelType"/> belonging to the <paramref name="hydraulicBoundaryLocationCalculations"/>.</param>
        /// <param name="targetProbability">The target probability belonging to the <paramref name="hydraulicBoundaryLocationCalculations"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> or <paramref name="hydraulicBoundaryLocationCalculations"/> is <c>null</c>.</exception>
        public SelectableTargetProbability(IAssessmentSection assessmentSection,
                                           IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations,
                                           WaveConditionsInputWaterLevelType waterLevelType, double targetProbability)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (hydraulicBoundaryLocationCalculations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocationCalculations));
            }

            this.assessmentSection = assessmentSection;
            HydraulicBoundaryLocationCalculations = hydraulicBoundaryLocationCalculations;
            WaterLevelType = waterLevelType;
            TargetProbability = targetProbability;
        }

        /// <summary>
        /// Gets the collection of <see cref="HydraulicBoundaryLocationCalculation"/>.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> HydraulicBoundaryLocationCalculations { get; }

        /// <summary>
        /// Gets the <see cref="WaveConditionsInputWaterLevelType"/>.
        /// </summary>
        public WaveConditionsInputWaterLevelType WaterLevelType { get; }

        /// <summary>
        /// Gets the target probability.
        /// </summary>
        public double TargetProbability { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SelectableTargetProbability) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = WaterLevelType.GetHashCode();
                hashCode = (hashCode * 397) ^ HydraulicBoundaryLocationCalculations.GetHashCode();
                hashCode = (hashCode * 397) ^ TargetProbability.GetHashCode();
                hashCode = (hashCode * 397) ^ assessmentSection.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(HydraulicBoundaryLocationCalculations,
                                                                                                                assessmentSection);
        }

        private bool Equals(SelectableTargetProbability other)
        {
            return ReferenceEquals(assessmentSection, other.assessmentSection)
                   && (ReferenceEquals(HydraulicBoundaryLocationCalculations, other.HydraulicBoundaryLocationCalculations)
                       || WaterLevelType == other.WaterLevelType
                       && Math.Abs(TargetProbability - other.TargetProbability) < 1e-6);
        }
    }
}