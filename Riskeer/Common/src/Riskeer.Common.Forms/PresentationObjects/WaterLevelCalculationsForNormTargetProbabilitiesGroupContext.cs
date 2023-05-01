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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all water level calculations based on norm target probabilities.
    /// </summary>
    public class WaterLevelCalculationsForNormTargetProbabilitiesGroupContext : ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryDatabase>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaterLevelCalculationsForNormTargetProbabilitiesGroupContext"/>.
        /// </summary>
        /// <param name="wrappedData">The databases that the <see cref="WaterLevelCalculationsForNormTargetProbabilitiesGroupContext"/> belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the <see cref="WaterLevelCalculationsForNormTargetProbabilitiesGroupContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public WaterLevelCalculationsForNormTargetProbabilitiesGroupContext(ObservableList<HydraulicBoundaryDatabase> wrappedData,
                                                                            IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}