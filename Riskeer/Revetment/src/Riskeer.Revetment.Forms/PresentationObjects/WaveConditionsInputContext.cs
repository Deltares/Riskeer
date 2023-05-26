﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for wave conditions input.
    /// </summary>
    /// <typeparam name="T">The type of the wave conditions input.</typeparam>
    public abstract class WaveConditionsInputContext<T> : ObservableWrappedObjectContextBase<T>
        where T : WaveConditionsInput
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WaveConditionsInputContext{T}"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        /// <param name="calculation">The calculation having <paramref name="wrappedData"/> as input.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected WaveConditionsInputContext(T wrappedData,
                                             ICalculation<T> calculation,
                                             IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Calculation = calculation;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the calculation containing the wrapped wave conditions input.
        /// </summary>
        public ICalculation<T> Calculation { get; }

        /// <summary>
        /// Gets the assessment section the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the hydraulic boundary locations.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
        {
            get
            {
                return AssessmentSection.HydraulicBoundaryData.GetLocations();
            }
        }

        /// <summary>
        /// Gets the foreshore profiles.
        /// </summary>
        public abstract IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}