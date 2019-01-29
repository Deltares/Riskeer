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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Data;

namespace Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an enumeration of <see cref="HydraulicBoundaryLocationCalculation"/>
    /// with a wave height calculation result for a given norm.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightCalculationsContext : WaveHeightCalculationsContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightCalculationsContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculations the context belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section the context belongs to.</param>
        /// <param name="getNormFunc"><see cref="Func{TResult}"/> for obtaining the norm to use during calculations.</param>
        /// <param name="categoryBoundaryName">The name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/>, <paramref name="failureMechanism"/>,
        /// <paramref name="assessmentSection"/> or <paramref name="getNormFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        public GrassCoverErosionOutwardsWaveHeightCalculationsContext(IObservableEnumerable<HydraulicBoundaryLocationCalculation> wrappedData,
                                                                      GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                      IAssessmentSection assessmentSection,
                                                                      Func<double> getNormFunc,
                                                                      string categoryBoundaryName)
            : base(wrappedData, assessmentSection, getNormFunc, categoryBoundaryName)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the grass cover erosion outwards failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; }
    }
}