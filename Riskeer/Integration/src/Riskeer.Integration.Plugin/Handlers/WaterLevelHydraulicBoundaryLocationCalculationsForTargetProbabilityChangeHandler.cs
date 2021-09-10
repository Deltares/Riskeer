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
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Service;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability.TargetProbability"/>
    /// value of a <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> and clearing all dependent data.
    /// </summary>
    public class WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler : HydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler 
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The calculations to change the target probability for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the that contains the <paramref name="calculationsForTargetProbability"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaterLevelHydraulicBoundaryLocationCalculationsForTargetProbabilityChangeHandler(
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability, IAssessmentSection assessmentSection)
            : base(calculationsForTargetProbability)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        protected override IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationDependentOutput()
        {
            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(base.ClearHydraulicBoundaryLocationCalculationDependentOutput());
            affectedObjects.AddRange(RiskeerDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(assessmentSection, CalculationsForTargetProbability));
            return affectedObjects;
        }
    }
}