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
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure multiple enumerations of <see cref="HydraulicBoundaryLocation"/> 
    /// with a wave height calculation result.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext
        : WaveHeightCalculationsGroupContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext"/>.
        /// </summary>
        /// <param name="wrappedData">The locations the context belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the context belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(ObservableList<HydraulicBoundaryLocation> wrappedData,
                                                                           GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                           IAssessmentSection assessmentSection)
            : base(wrappedData, assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the failure mechanism the context belongs to.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; }
    }
}