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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for wave conditions input of the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsInputContext : WaveConditionsInputContext<GrassCoverErosionOutwardsWaveConditionsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        /// <param name="calculation">The calculation having <paramref name="wrappedData"/> as input.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the context belongs to.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles of the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveConditionsInputContext(GrassCoverErosionOutwardsWaveConditionsInput wrappedData,
                                                                   ICalculation<GrassCoverErosionOutwardsWaveConditionsInput> calculation,
                                                                   IAssessmentSection assessmentSection,
                                                                   IEnumerable<ForeshoreProfile> foreshoreProfiles)
            : base(wrappedData, calculation, assessmentSection)
        {
            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }

            ForeshoreProfiles = foreshoreProfiles;
        }

        public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}