// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Service;

namespace Ringtoets.Integration.Service
{
    /// <summary>
    /// Service for synchronizing ringtoets.
    /// </summary>
    public static class RingtoetsDataSynchronizationService
    {
        /// <summary>
        /// Clears all the output data within the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to clear the data for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static void ClearAssessmentSectionData(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            ClearHydraulicBoundaryLocationOutput(assessmentSection.HydraulicBoundaryDatabase);
            ClearFailureMechanismCalculationOutputs(assessmentSection);
        }

        private static void ClearHydraulicBoundaryLocationOutput(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                return;
            }

            foreach (var hydraulicBoundaryLocation in hydraulicBoundaryDatabase.Locations) 
            {
                hydraulicBoundaryLocation.DesignWaterLevel = double.NaN;
                hydraulicBoundaryLocation.WaveHeight = double.NaN;
            }
        }

        private static void ClearFailureMechanismCalculationOutputs(AssessmentSection assessmentSection)
        {
            PipingDataSynchronizationService.ClearAllCalculationOutput(assessmentSection.PipingFailureMechanism);
            GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(assessmentSection.GrassCoverErosionInwards);
            HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(assessmentSection.HeightStructures);
        }
    }
}