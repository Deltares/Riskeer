// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Service;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Service;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Service;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Service;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Service;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Service;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Service;
using Riskeer.Revetment.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Service;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Service;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Service;

namespace Riskeer.Integration.Service
{
    /// <summary>
    /// Service for synchronizing Riskeer items.
    /// </summary>
    public static class RiskeerDataSynchronizationService
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
                switch (failureMechanism)
                {
                    case PipingFailureMechanism pipingFailureMechanism:
                        changedObservables.AddRange(PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(pipingFailureMechanism));
                        break;
                    case GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism:
                        changedObservables.AddRange(GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(grassCoverErosionInwardsFailureMechanism));
                        break;
                    case StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism:
                        changedObservables.AddRange(StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(stabilityStoneCoverFailureMechanism));
                        break;
                    case WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism:
                        changedObservables.AddRange(WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(waveImpactAsphaltCoverFailureMechanism));
                        break;
                    case GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism:
                        changedObservables.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(grassCoverErosionOutwardsFailureMechanism));
                        break;
                    case HeightStructuresFailureMechanism heightStructuresFailureMechanism:
                        changedObservables.AddRange(HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(heightStructuresFailureMechanism));
                        break;
                    case ClosingStructuresFailureMechanism closingStructuresFailureMechanism:
                        changedObservables.AddRange(ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(closingStructuresFailureMechanism));
                        break;
                    case StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism:
                        changedObservables.AddRange(StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(stabilityPointStructuresFailureMechanism));
                        break;
                    case MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism:
                        changedObservables.AddRange(MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(macroStabilityInwardsFailureMechanism));
                        break;
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

            var changedObservables = new List<IObservable>();

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                switch (failureMechanism)
                {
                    case PipingFailureMechanism pipingFailureMechanism:
                        changedObservables.AddRange(PipingDataSynchronizationService.ClearAllSemiProbabilisticCalculationOutputWithoutManualAssessmentLevel(pipingFailureMechanism));
                        changedObservables.AddRange(PipingDataSynchronizationService.ClearAllProbabilisticCalculationOutput(pipingFailureMechanism));
                        break;
                    case GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism:
                        changedObservables.AddRange(GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(grassCoverErosionInwardsFailureMechanism));
                        break;
                    case StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism:
                        changedObservables.AddRange(StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(stabilityStoneCoverFailureMechanism));
                        break;
                    case WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism:
                        changedObservables.AddRange(WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(waveImpactAsphaltCoverFailureMechanism));
                        break;
                    case GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism:
                        changedObservables.AddRange(GrassCoverErosionOutwardsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(grassCoverErosionOutwardsFailureMechanism));
                        break;
                    case HeightStructuresFailureMechanism heightStructuresFailureMechanism:
                        changedObservables.AddRange(HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(heightStructuresFailureMechanism));
                        break;
                    case ClosingStructuresFailureMechanism closingStructuresFailureMechanism:
                        changedObservables.AddRange(ClosingStructuresDataSynchronizationService.ClearAllCalculationOutput(closingStructuresFailureMechanism));
                        break;
                    case StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism:
                        changedObservables.AddRange(StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(stabilityPointStructuresFailureMechanism));
                        break;
                    case MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism:
                        changedObservables.AddRange(MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputWithoutManualAssessmentLevel(macroStabilityInwardsFailureMechanism));
                        break;
                }
            }

            return changedObservables;
        }

        /// <summary>
        /// Clears the output of all semi probabilistic calculations in the <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of calculations which are affected by clearing the output.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearAllSemiProbabilisticCalculationOutput(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IEnumerable<IFailureMechanism> failureMechanisms = assessmentSection.GetFailureMechanisms();

            var changedObservables = new List<IObservable>();
            changedObservables.AddRange(PipingDataSynchronizationService.ClearAllSemiProbabilisticCalculationOutputWithoutManualAssessmentLevel(
                                            failureMechanisms.OfType<PipingFailureMechanism>()
                                                             .Single()));
            changedObservables.AddRange(MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputWithoutManualAssessmentLevel(
                                            failureMechanisms.OfType<MacroStabilityInwardsFailureMechanism>()
                                                             .Single()));
            return changedObservables;
        }

        /// <summary>
        /// Clears the hydraulic boundary location calculation output that is contained 
        /// within specific failure mechanisms of the <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the hydraulic boundary 
        /// location calculations and failure mechanisms at stake.</param>
        /// <returns>All objects affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationOutput(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(DuneErosionDataSynchronizationService.ClearDuneLocationCalculationsOutput(assessmentSection.GetFailureMechanisms()
                                                                                                                                .OfType<DuneErosionFailureMechanism>()
                                                                                                                                .Single()));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         assessmentSection.WaterLevelCalculationsForSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(
                                         assessmentSection.WaterLevelCalculationsForLowerLimitNorm));

            foreach (IEnumerable<HydraulicBoundaryLocationCalculation> calculations in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                        .Select(c => c.HydraulicBoundaryLocationCalculations))
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculations));
            }

            foreach (IEnumerable<HydraulicBoundaryLocationCalculation> calculations in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                                                        .Select(c => c.HydraulicBoundaryLocationCalculations))
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculations));
            }

            return affectedObjects.ToArray();
        }

        /// <summary>
        /// Clears the hydraulic boundary location calculation output that is contained within a
        /// <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>
        /// which contains the hydraulic boundary location calculations at stake.</param>
        /// <returns>All objects affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationsForTargetProbability"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationOutput(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability)
        {
            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            return RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations);
        }

        /// <summary>
        /// Clears all illustration point results of the norm target probability based water level calculations.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to clear the illustration point results for.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         assessmentSection.WaterLevelCalculationsForSignalingNorm));
            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                         assessmentSection.WaterLevelCalculationsForLowerLimitNorm));
            return affectedObjects;
        }

        /// <summary>
        /// Clears the illustration point results of the user defined target probability based water level calculations.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to clear the illustration point results for.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                             element.HydraulicBoundaryLocationCalculations));
            }

            return affectedObjects;
        }

        /// <summary>
        /// Clears the illustration point results of the user defined target probability based wave height calculations.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to clear the illustration point results for.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(
                                             element.HydraulicBoundaryLocationCalculations));
            }

            return affectedObjects;
        }

        /// <summary>
        /// Clears the illustration point results for all water level and wave height calculations.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to clear the illustration point results for.</param>
        /// <returns>All objects that are affected by the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearIllustrationPointResultsForWaterLevelAndWaveHeightCalculations(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(ClearIllustrationPointResultsOfWaterLevelCalculationsForNormTargetProbabilities(assessmentSection));
            affectedObjects.AddRange(ClearIllustrationPointResultsOfWaterLevelCalculationsForUserDefinedTargetProbabilities(assessmentSection));
            affectedObjects.AddRange(ClearIllustrationPointResultsOfWaveHeightCalculationsForUserDefinedTargetProbabilities(assessmentSection));
            return affectedObjects;
        }

        /// <summary>
        /// Clears the reference line and all data that depends on it, either directly or indirectly.
        /// </summary>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <returns>The results of the clear action.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public static ClearResults ClearReferenceLineDependentData(IAssessmentSection assessmentSection)
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
            changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearForeshoreProfile<HeightStructuresInput, HeightStructure>(calculations, profile));

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
            changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearForeshoreProfile<ClosingStructuresInput, ClosingStructure>(calculations, profile));

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
            changedObservables.AddRange(RiskeerCommonDataSynchronizationService.ClearForeshoreProfile<StabilityPointStructuresInput, StabilityPointStructure>(calculations, profile));

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
                foreach (IObservable calculationWithRemovedOutput in RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation))
                {
                    changedObservables.Add(calculationWithRemovedOutput);
                }

                calculation.InputParameters.DikeProfile = null;
                changedObservables.Add(calculation.InputParameters);
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
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));
            }

            foreshoreProfiles.Clear();
            affectedObjects.Add(foreshoreProfiles);
            return affectedObjects;
        }

        private static ClearResults GetClearResultsForFailureMechanism(IFailureMechanism failureMechanism)
        {
            switch (failureMechanism)
            {
                case PipingFailureMechanism pipingFailureMechanism:
                    return PipingDataSynchronizationService.ClearReferenceLineDependentData(pipingFailureMechanism);
                case MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism:
                    return MacroStabilityInwardsDataSynchronizationService.ClearReferenceLineDependentData(macroStabilityInwardsFailureMechanism);
                case GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism:
                    return GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(grassCoverErosionInwardsFailureMechanism);
                case StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism:
                    return StabilityStoneCoverDataSynchronizationService.ClearReferenceLineDependentData(stabilityStoneCoverFailureMechanism);
                case WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism:
                    return WaveImpactAsphaltCoverDataSynchronizationService.ClearReferenceLineDependentData(waveImpactAsphaltCoverFailureMechanism);
                case GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism:
                    return GrassCoverErosionOutwardsDataSynchronizationService.ClearReferenceLineDependentData(grassCoverErosionOutwardsFailureMechanism);
                case HeightStructuresFailureMechanism heightStructuresFailureMechanism:
                    return HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(heightStructuresFailureMechanism);
                case ClosingStructuresFailureMechanism closingStructuresFailureMechanism:
                    return ClosingStructuresDataSynchronizationService.ClearReferenceLineDependentData(closingStructuresFailureMechanism);
                case StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism:
                    return StabilityPointStructuresDataSynchronizationService.ClearReferenceLineDependentData(stabilityPointStructuresFailureMechanism);
                default:
                    return ClearReferenceLineDependentDataForFailureMechanism(failureMechanism);
            }
        }

        private static ClearResults ClearReferenceLineDependentDataForFailureMechanism(IFailureMechanism failureMechanism)
        {
            var removedObjects = new List<object>();
            var changedObjects = new List<IObservable>();

            removedObjects.AddRange(failureMechanism.Sections);
            changedObjects.Add(failureMechanism);

            if (failureMechanism is IHasSectionResults<FailureMechanismSectionResult> failureMechanismWithSectionResults)
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
                foreach (IObservable calculationWithRemovedOutput in RiskeerCommonDataSynchronizationService.ClearCalculationOutput(input.Item1))
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