﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion outwards data.
    /// </summary>
    public static class GrassCoverErosionOutwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output of the given <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
        /// to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        /// <returns>All objects that have been changed.</returns>
        public static IEnumerable<IObservable> ClearWaveConditionsCalculationOutput(GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (calculation.HasOutput)
            {
                calculation.ClearOutput();
                return new[]
                {
                    calculation
                };
            }

            return Enumerable.Empty<IObservable>();
        }

        /// <summary>
        /// Clears the <see cref="HydraulicBoundaryLocation"/> and output for all the wave conditions calculations
        /// in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects which are affected by removal of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedItems = new List<IObservable>();
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in failureMechanism.Calculations.Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>())
            {
                affectedItems.AddRange(ClearWaveConditionsCalculationOutput(calculation)
                                           .Concat(ClearHydraulicBoundaryLocation(calculation.InputParameters)));
            }

            return affectedItems;
        }

        /// <summary>
        /// Clears the output for all calculations in the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by
        /// clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllWaveConditionsCalculationOutput(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return failureMechanism.Calculations
                                   .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                   .SelectMany(ClearWaveConditionsCalculationOutput)
                                   .ToArray();
        }

        /// <summary>
        /// Clears all data dependent, either directly or indirectly, on the parent reference line.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be cleared.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLineDependentData(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var changedObjects = new List<IObservable>();
            object[] removedObjects = failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive().OfType<object>()
                                                      .Concat(failureMechanism.ForeshoreProfiles)
                                                      .ToArray();

            changedObjects.Add(failureMechanism);

            failureMechanism.WaveConditionsCalculationGroup.Children.Clear();
            changedObjects.Add(failureMechanism.WaveConditionsCalculationGroup);

            failureMechanism.ForeshoreProfiles.Clear();
            changedObjects.Add(failureMechanism.ForeshoreProfiles);

            return new ClearResults(changedObjects, removedObjects);
        }

        /// <summary>
        /// Clears the hydraulic boundary location calculation output that is contained within the grass cover erosion outwards failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to clear the hydraulic boundary location calculation output for.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationOutput(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm));

            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm));

            return affectedObjects;
        }

        /// <summary>
        /// Clears all the illustration point results for the design water level calculations that are relevant for the grass cover erosion
        /// outwards failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to clear the
        /// illustration point results for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsForDesignWaterLevelCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                                            IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         assessmentSection.WaterLevelCalculationsForLowerLimitNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm));
            return affectedObjects;
        }

        /// <summary>
        /// Clears all the illustration point results for the wave height calculations that are relevant for the grass cover erosion
        /// outwards failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to clear the
        /// illustration point results for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsForWaveHeightCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                                      IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         assessmentSection.WaveHeightCalculationsForLowerLimitNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm));
            return affectedObjects;
        }

        /// <summary>
        /// Clears all the illustration point results for the design water level and wave height calculations that are relevant for the grass cover erosion
        /// outwards failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to clear the
        /// illustration point results for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="failureMechanism"/> belongs to.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                                                         IAssessmentSection assessmentSection)
        {
            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(ClearIllustrationPointResultsForWaveHeightCalculations(failureMechanism, assessmentSection));
            affectedObjects.AddRange(ClearIllustrationPointResultsForDesignWaterLevelCalculations(failureMechanism, assessmentSection));

            return affectedObjects;
        }

        private static IEnumerable<IObservable> ClearHydraulicBoundaryLocation(WaveConditionsInput input)
        {
            if (input.HydraulicBoundaryLocation != null)
            {
                input.HydraulicBoundaryLocation = null;
                return new[]
                {
                    input
                };
            }

            return Enumerable.Empty<IObservable>();
        }
    }
}