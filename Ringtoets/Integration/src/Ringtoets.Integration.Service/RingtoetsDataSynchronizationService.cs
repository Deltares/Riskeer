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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Service;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Service;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Service;

namespace Ringtoets.Integration.Service
{
    /// <summary>
    /// Service for synchronizing ringtoets.
    /// </summary>
    public static class RingtoetsDataSynchronizationService
    {
        /// <summary>
        /// Clears all the output data and hydraulic boundary locations within the <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to clear the data for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// removing data.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<ICalculation> ClearAllCalculationOutputAndHydraulicBoundaryLocations(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            var affectedItems = new List<ICalculation>();

            foreach (var failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;

                if (pipingFailureMechanism != null)
                {
                    affectedItems.AddRange(PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(pipingFailureMechanism));
                }
                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    affectedItems.AddRange(GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(grassCoverErosionInwardsFailureMechanism));
                }
                if (stabilityStoneCoverFailureMechanism != null)
                {
                    affectedItems.AddRange(StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(stabilityStoneCoverFailureMechanism));
                }
                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    affectedItems.AddRange(WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(waveImpactAsphaltCoverFailureMechanism));
                }
                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    affectedItems.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(grassCoverErosionOutwardsFailureMechanism));
                }
                if (heightStructuresFailureMechanism != null)
                {
                    affectedItems.AddRange(HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(heightStructuresFailureMechanism));
                }
                if (closingStructuresFailureMechanism != null)
                {
                    affectedItems.AddRange(ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(closingStructuresFailureMechanism));
                }
                if (stabilityPointStructuresFailureMechanism != null)
                {
                    affectedItems.AddRange(StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(stabilityPointStructuresFailureMechanism));
                }
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output of all calculations in the <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<ICalculation> ClearFailureMechanismCalculationOutputs(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            var affectedItems = new List<ICalculation>();

            foreach (var failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;

                if (pipingFailureMechanism != null)
                {
                    affectedItems.AddRange(PipingDataSynchronizationService.ClearAllCalculationOutput(pipingFailureMechanism));
                }
                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    affectedItems.AddRange(GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(grassCoverErosionInwardsFailureMechanism));
                }
                if (stabilityStoneCoverFailureMechanism != null)
                {
                    affectedItems.AddRange(StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(stabilityStoneCoverFailureMechanism));
                }
                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    affectedItems.AddRange(WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(waveImpactAsphaltCoverFailureMechanism));
                }
                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    affectedItems.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(grassCoverErosionOutwardsFailureMechanism));
                }
                if (heightStructuresFailureMechanism != null)
                {
                    affectedItems.AddRange(HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(heightStructuresFailureMechanism));
                }
                if (closingStructuresFailureMechanism != null)
                {
                    affectedItems.AddRange(ClosingStructuresDataSynchronizationService.ClearAllCalculationOutput(closingStructuresFailureMechanism));
                }
                if (stabilityPointStructuresFailureMechanism != null)
                {
                    affectedItems.AddRange(StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(stabilityPointStructuresFailureMechanism));
                }
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output of the hydraulic boundary locations within the <paramref name="hydraulicBoundaryDatabase"/>
        /// and <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> wich contains the locations.</param>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> which contains the locations.</param>
        /// <returns><c>true</c> when one or multiple locations are affected by clearing the output. <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/> 
        /// or <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static bool ClearHydraulicBoundaryLocationOutput(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryDatabase");
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var locationsAffected = GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(failureMechanism.HydraulicBoundaryLocations);

            foreach (var hydraulicBoundaryLocation in hydraulicBoundaryDatabase.Locations
                                                                               .Where(hydraulicBoundaryLocation =>
                                                                                      !double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel) ||
                                                                                      !double.IsNaN(hydraulicBoundaryLocation.WaveHeight)))
            {
                hydraulicBoundaryLocation.DesignWaterLevel = RoundedDouble.NaN;
                hydraulicBoundaryLocation.WaveHeight = RoundedDouble.NaN;
                hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence = CalculationConvergence.NotCalculated;
                hydraulicBoundaryLocation.WaveHeightCalculationConvergence = CalculationConvergence.NotCalculated;
                locationsAffected = true;
            }

            return locationsAffected;
        }
    }
}