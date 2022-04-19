﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all water level calculations based on a user defined target probability.
    /// </summary>
    public class WaterLevelCalculationsForUserDefinedTargetProbabilityContext : HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaterLevelCalculationsForUserDefinedTargetProbabilityContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculations wrapped by the <see cref="WaterLevelCalculationsForUserDefinedTargetProbabilityContext"/>.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the <see cref="WaterLevelCalculationsForUserDefinedTargetProbabilityContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaterLevelCalculationsForUserDefinedTargetProbabilityContext(HydraulicBoundaryLocationCalculationsForTargetProbability wrappedData,
                                                                            IAssessmentSection assessmentSection)
            : base(wrappedData, assessmentSection) {}

        protected override IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> LocationCalculationsEnumerationToObserve => AssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities;
    }
}