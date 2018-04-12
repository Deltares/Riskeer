﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Service;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.GrassCoverErosionInwards.Util;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;
using Ringtoets.Revetment.Data;
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
        public static IEnumerable<IObservable> ClearAllCalculationOutputAndHydraulicBoundaryLocations(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var changedObservables = new List<IObservable>();

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var macroStabilityInwardsFailureMechanism = failureMechanism as MacroStabilityInwardsFailureMechanism;

                if (pipingFailureMechanism != null)
                {
                    changedObservables.AddRange(PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(pipingFailureMechanism));
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    changedObservables.AddRange(GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(grassCoverErosionInwardsFailureMechanism));
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    changedObservables.AddRange(StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(stabilityStoneCoverFailureMechanism));
                }

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    changedObservables.AddRange(WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(waveImpactAsphaltCoverFailureMechanism));
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    changedObservables.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(grassCoverErosionOutwardsFailureMechanism));
                }

                if (heightStructuresFailureMechanism != null)
                {
                    changedObservables.AddRange(HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(heightStructuresFailureMechanism));
                }

                if (closingStructuresFailureMechanism != null)
                {
                    changedObservables.AddRange(ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(closingStructuresFailureMechanism));
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    changedObservables.AddRange(StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(stabilityPointStructuresFailureMechanism));
                }

                if (macroStabilityInwardsFailureMechanism != null)
                {
                    changedObservables.AddRange(MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(macroStabilityInwardsFailureMechanism));
                }
            }

            return changedObservables;
        }

        /// <summary>
        /// Clears the output of all calculations in the <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearFailureMechanismCalculationOutputs(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return ClearFailureMechanismCalculationOutputs(assessmentSection.GetFailureMechanisms());
        }

        /// <summary>
        /// Clears the output of all calculations of the given <paramref name="failureMechanisms"/>.
        /// </summary>
        /// <param name="failureMechanisms">The failure mechanisms that contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations that are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearFailureMechanismCalculationOutputs(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            var changedObservables = new List<IObservable>();

            foreach (IFailureMechanism failureMechanism in failureMechanisms)
            {
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var macroStabilityInwardsFailureMechanism = failureMechanism as MacroStabilityInwardsFailureMechanism;

                if (pipingFailureMechanism != null)
                {
                    changedObservables.AddRange(PipingDataSynchronizationService.ClearAllCalculationOutput(pipingFailureMechanism));
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    changedObservables.AddRange(GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(grassCoverErosionInwardsFailureMechanism));
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    changedObservables.AddRange(StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(stabilityStoneCoverFailureMechanism));
                }

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    changedObservables.AddRange(WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(waveImpactAsphaltCoverFailureMechanism));
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    changedObservables.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(grassCoverErosionOutwardsFailureMechanism));
                }

                if (heightStructuresFailureMechanism != null)
                {
                    changedObservables.AddRange(HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(heightStructuresFailureMechanism));
                }

                if (closingStructuresFailureMechanism != null)
                {
                    changedObservables.AddRange(ClosingStructuresDataSynchronizationService.ClearAllCalculationOutput(closingStructuresFailureMechanism));
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    changedObservables.AddRange(StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(stabilityPointStructuresFailureMechanism));
                }

                if (macroStabilityInwardsFailureMechanism != null)
                {
                    changedObservables.AddRange(MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutput(macroStabilityInwardsFailureMechanism));
                }
            }

            return changedObservables;
        }

        /// <summary>
        /// Clears the output of the hydraulic boundary locations within the <paramref name="hydraulicBoundaryDatabase"/> and the 
        /// hydraulic boundary locations that are contained within specific failure mechanisms of the <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> which contains the locations.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the failure mechanisms.</param>
        /// <returns>All objects affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationOutput(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, IAssessmentSection assessmentSection)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(assessmentSection));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaterLevelCalculationsForSignalingNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaterLevelCalculationsForLowerLimitNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaveHeightCalculationsForSignalingNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaveHeightCalculationsForLowerLimitNorm));
            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm));

            return affectedObjects.ToArray();
        }

        /// <summary>
        /// Clears the output of the hydraulic boundary locations that are contained within specific failure mechanisms 
        /// of the <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the failure mechanisms.</param>
        /// <returns>All objects affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(assessmentSection.GetFailureMechanisms());
        }

        /// <summary>
        /// Clears the output of the hydraulic boundary locations that are contained within specific <paramref name="failureMechanisms"/>.
        /// </summary>
        /// <param name="failureMechanisms">The failure mechanisms to clear the hydraulic boundary locations for.</param>
        /// <returns>All objects affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            var changedObservables = new List<IObservable>();
            foreach (IFailureMechanism failureMechanism in failureMechanisms)
            {
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var duneErosionFailureMechanism = failureMechanism as DuneErosionFailureMechanism;

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    IEnumerable<IObservable> affectedHydraulicBoundaryLocations =
                        GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputs(grassCoverErosionOutwardsFailureMechanism);
                    changedObservables.AddRange(affectedHydraulicBoundaryLocations);
                }

                if (duneErosionFailureMechanism != null)
                {
                    IEnumerable<IObservable> affectedDuneLocations =
                        DuneErosionDataSynchronizationService.ClearDuneLocationOutput(duneErosionFailureMechanism.DuneLocations)
                                                             .Concat(DuneErosionDataSynchronizationService.ClearDuneCalculationOutputs(duneErosionFailureMechanism));
                    changedObservables.AddRange(affectedDuneLocations);
                }
            }

            return changedObservables;
        }

        /// <summary>
        /// Clears the reference line and all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLine(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var changedObjects = new List<IObservable>();
            var removedObjects = new List<object>();

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                ClearResults results = GetClearResultsForFailureMechanism(failureMechanism);

                changedObjects.AddRange(results.ChangedObjects);
                removedObjects.AddRange(results.RemovedObjects);
            }

            if (assessmentSection.ReferenceLine != null)
            {
                removedObjects.Add(assessmentSection.ReferenceLine);
                assessmentSection.ReferenceLine = null;
            }

            changedObjects.Add(assessmentSection);

            return new ClearResults(changedObjects, removedObjects);
        }

        /// <summary>
        /// Removes a given <see cref="ForeshoreProfile"/> from the <see cref="HeightStructuresFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveForeshoreProfile(HeightStructuresFailureMechanism failureMechanism, ForeshoreProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new List<IObservable>();
            IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations = failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>();
            changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearForeshoreProfile<HeightStructuresInput, HeightStructure>(calculations, profile));

            failureMechanism.ForeshoreProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.ForeshoreProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="ForeshoreProfile"/> from the <see cref="ClosingStructuresFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveForeshoreProfile(ClosingStructuresFailureMechanism failureMechanism, ForeshoreProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new List<IObservable>();
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations = failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>();
            changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearForeshoreProfile<ClosingStructuresInput, ClosingStructure>(calculations, profile));

            failureMechanism.ForeshoreProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.ForeshoreProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="ForeshoreProfile"/> from the <see cref="StabilityPointStructuresFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveForeshoreProfile(StabilityPointStructuresFailureMechanism failureMechanism, ForeshoreProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new List<IObservable>();
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations = failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>();
            changedObservables.AddRange(RingtoetsCommonDataSynchronizationService.ClearForeshoreProfile<StabilityPointStructuresInput, StabilityPointStructure>(calculations, profile));

            failureMechanism.ForeshoreProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.ForeshoreProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="ForeshoreProfile"/> from the <see cref="StabilityStoneCoverFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveForeshoreProfile(StabilityStoneCoverFailureMechanism failureMechanism, ForeshoreProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new List<IObservable>();
            Tuple<ICalculation, WaveConditionsInput>[] calculationsWithInput = failureMechanism.Calculations
                                                                                               .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                                                               .Select(c => Tuple.Create<ICalculation, WaveConditionsInput>(c, c.InputParameters))
                                                                                               .ToArray();
            changedObservables.AddRange(OnWaveConditionsInputForeshoreProfileRemoved(profile, calculationsWithInput));

            failureMechanism.ForeshoreProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.ForeshoreProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="ForeshoreProfile"/> from the <see cref="WaveImpactAsphaltCoverFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveForeshoreProfile(WaveImpactAsphaltCoverFailureMechanism failureMechanism, ForeshoreProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new List<IObservable>();
            Tuple<ICalculation, WaveConditionsInput>[] calculationsWithInput = failureMechanism.Calculations
                                                                                               .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                                                               .Select(c => Tuple.Create<ICalculation, WaveConditionsInput>(c, c.InputParameters))
                                                                                               .ToArray();
            changedObservables.AddRange(OnWaveConditionsInputForeshoreProfileRemoved(profile, calculationsWithInput));

            failureMechanism.ForeshoreProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.ForeshoreProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="ForeshoreProfile"/> from the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveForeshoreProfile(GrassCoverErosionOutwardsFailureMechanism failureMechanism, ForeshoreProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new List<IObservable>();
            Tuple<ICalculation, WaveConditionsInput>[] calculationsWithInput = failureMechanism.Calculations
                                                                                               .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                               .Select(c => Tuple.Create<ICalculation, WaveConditionsInput>(c, c.InputParameters))
                                                                                               .ToArray();
            changedObservables.AddRange(OnWaveConditionsInputForeshoreProfileRemoved(profile, calculationsWithInput));

            failureMechanism.ForeshoreProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.ForeshoreProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes a given <see cref="DikeProfile"/> from the <see cref="GrassCoverErosionInwardsFailureMechanism"/>
        /// and clears all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism containing <paramref name="profile"/>.</param>
        /// <param name="profile">The profile residing in <paramref name="failureMechanism"/>
        /// that should be removed.</param>
        /// <returns>All observable objects affected by this method.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveDikeProfile(GrassCoverErosionInwardsFailureMechanism failureMechanism, DikeProfile profile)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var changedObservables = new HashSet<IObservable>();
            GrassCoverErosionInwardsCalculation[] calculations = failureMechanism.Calculations
                                                                                 .Cast<GrassCoverErosionInwardsCalculation>()
                                                                                 .ToArray();
            GrassCoverErosionInwardsCalculation[] calculationWithRemovedDikeProfile = calculations
                                                                                      .Where(c => ReferenceEquals(c.InputParameters.DikeProfile, profile))
                                                                                      .ToArray();
            foreach (GrassCoverErosionInwardsCalculation calculation in calculationWithRemovedDikeProfile)
            {
                foreach (IObservable calculationWithRemovedOutput in RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation))
                {
                    changedObservables.Add(calculationWithRemovedOutput);
                }

                calculation.InputParameters.DikeProfile = null;
                changedObservables.Add(calculation.InputParameters);
            }

            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> changedSectionResults =
                GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(failureMechanism.SectionResults, calculations);
            foreach (GrassCoverErosionInwardsFailureMechanismSectionResult result in changedSectionResults)
            {
                changedObservables.Add(result);
            }

            failureMechanism.DikeProfiles.Remove(profile);
            changedObservables.Add(failureMechanism.DikeProfiles);

            return changedObservables;
        }

        /// <summary>
        /// Removes all <see cref="ForeshoreProfile"/> from the <paramref name="calculations"/> 
        /// and the <paramref name="foreshoreProfiles"/> in addition to clearing all data that depends 
        /// on it, either directly or indirectly.
        /// </summary>
        /// <param name="calculations">The calculations that need to be updated.</param>
        /// <param name="foreshoreProfiles">The collection containing the foreshore profiles.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with affected objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is 
        /// <c>null</c>.</exception>
        public static IEnumerable<IObservable> RemoveAllForeshoreProfiles<T>(IEnumerable<ICalculation<T>> calculations,
                                                                             ForeshoreProfileCollection foreshoreProfiles)
            where T : ICalculationInput
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }

            IEnumerable<ICalculation<T>> calculationsWithForeshoreProfiles =
                calculations.Where(calc => ((IHasForeshoreProfile) calc.InputParameters)
                                           .ForeshoreProfile != null);

            var affectedObjects = new List<IObservable>();
            foreach (ICalculation<T> calculation in calculationsWithForeshoreProfiles)
            {
                ((IHasForeshoreProfile) calculation.InputParameters).ForeshoreProfile = null;
                affectedObjects.Add(calculation.InputParameters);
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            foreshoreProfiles.Clear();
            affectedObjects.Add(foreshoreProfiles);
            return affectedObjects;
        }

        private static ClearResults GetClearResultsForFailureMechanism(IFailureMechanism failureMechanism)
        {
            var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
            if (pipingFailureMechanism != null)
            {
                return PipingDataSynchronizationService.ClearReferenceLineDependentData(pipingFailureMechanism);
            }

            var macroStabilityInwardsFailureMechanism = failureMechanism as MacroStabilityInwardsFailureMechanism;
            if (macroStabilityInwardsFailureMechanism != null)
            {
                return MacroStabilityInwardsDataSynchronizationService.ClearReferenceLineDependentData(macroStabilityInwardsFailureMechanism);
            }

            var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
            if (grassCoverErosionInwardsFailureMechanism != null)
            {
                return GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(grassCoverErosionInwardsFailureMechanism);
            }

            var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;
            if (stabilityStoneCoverFailureMechanism != null)
            {
                return StabilityStoneCoverDataSynchronizationService.ClearReferenceLineDependentData(stabilityStoneCoverFailureMechanism);
            }

            var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
            if (waveImpactAsphaltCoverFailureMechanism != null)
            {
                return WaveImpactAsphaltCoverDataSynchronizationService.ClearReferenceLineDependentData(waveImpactAsphaltCoverFailureMechanism);
            }

            var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
            if (grassCoverErosionOutwardsFailureMechanism != null)
            {
                return GrassCoverErosionOutwardsDataSynchronizationService.ClearReferenceLineDependentData(grassCoverErosionOutwardsFailureMechanism);
            }

            var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
            if (heightStructuresFailureMechanism != null)
            {
                return HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(heightStructuresFailureMechanism);
            }

            var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
            if (closingStructuresFailureMechanism != null)
            {
                return ClosingStructuresDataSynchronizationService.ClearReferenceLineDependentData(closingStructuresFailureMechanism);
            }

            var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;
            if (stabilityPointStructuresFailureMechanism != null)
            {
                return StabilityPointStructuresDataSynchronizationService.ClearReferenceLineDependentData(stabilityPointStructuresFailureMechanism);
            }

            return ClearReferenceLineDependentData(failureMechanism);
        }

        private static ClearResults ClearReferenceLineDependentData(IFailureMechanism failureMechanism)
        {
            var removedObjects = new List<object>();
            var changedObjects = new List<IObservable>();

            removedObjects.AddRange(failureMechanism.Sections);
            changedObjects.Add(failureMechanism);

            var failureMechanismWithSectionResults = failureMechanism as IHasSectionResults<FailureMechanismSectionResult>;
            if (failureMechanismWithSectionResults != null)
            {
                removedObjects.AddRange(failureMechanismWithSectionResults.SectionResults);
                changedObjects.Add(failureMechanismWithSectionResults.SectionResults);
            }

            failureMechanism.ClearAllSections();

            return new ClearResults(changedObjects, removedObjects);
        }

        private static IEnumerable<IObservable> OnWaveConditionsInputForeshoreProfileRemoved(ForeshoreProfile profile, Tuple<ICalculation, WaveConditionsInput>[] calculationInputs)
        {
            var changedObservables = new List<IObservable>();
            foreach (Tuple<ICalculation, WaveConditionsInput> input in calculationInputs.Where(input => ReferenceEquals(input.Item2.ForeshoreProfile, profile)))
            {
                foreach (IObservable calculationWithRemovedOutput in RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(input.Item1))
                {
                    changedObservables.Add(calculationWithRemovedOutput);
                }

                input.Item2.ForeshoreProfile = null;
                changedObservables.Add(input.Item2);
            }

            return changedObservables;
        }
    }
}