// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Util.Extensions;
using Core.Gui.Forms.ViewHost;
using log4net;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Helpers;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.Revetment.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Plugin.Merge
{
    /// <summary>
    /// Class responsible for handling the merge of <see cref="AssessmentSection"/> data.
    /// </summary>
    public class AssessmentSectionMergeHandler : IAssessmentSectionMergeHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionMergeHandler));
        private readonly IDocumentViewController documentViewController;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeHandler"/>.
        /// </summary>
        /// <param name="documentViewController">The document view controller.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="documentViewController"/> is <c>mull</c>.</exception>
        public AssessmentSectionMergeHandler(IDocumentViewController documentViewController)
        {
            if (documentViewController == null)
            {
                throw new ArgumentNullException(nameof(documentViewController));
            }

            this.documentViewController = documentViewController;
        }

        public void PerformMerge(AssessmentSection targetAssessmentSection, AssessmentSectionMergeData mergeData,
                                 IHydraulicBoundaryDataUpdateHandler hydraulicBoundaryDataUpdateHandler)
        {
            if (targetAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(targetAssessmentSection));
            }

            if (mergeData == null)
            {
                throw new ArgumentNullException(nameof(mergeData));
            }

            if (hydraulicBoundaryDataUpdateHandler == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDataUpdateHandler));
            }

            ValidateMergeData(mergeData);

            IEnumerable<IObservable> changedObjects = MergeHydraulicBoundaryData(targetAssessmentSection, mergeData.AssessmentSection,
                                                                                 hydraulicBoundaryDataUpdateHandler);

            MergeFailureMechanisms(targetAssessmentSection, mergeData);
            MergeSpecificFailureMechanism(targetAssessmentSection, mergeData.MergeSpecificFailureMechanisms);

            AfterMerge(changedObjects);
        }

        /// <summary>
        /// Validates the <see cref="AssessmentSectionMergeData"/>.
        /// </summary>
        /// <param name="mergeData">The <see cref="AssessmentSectionMergeData"/> to validate.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mergeData"/> is invalid.</exception>
        private static void ValidateMergeData(AssessmentSectionMergeData mergeData)
        {
            AssessmentSection sourceAssessmentSection = mergeData.AssessmentSection;
            if (!mergeData.MergeSpecificFailureMechanisms.All(fp => sourceAssessmentSection.SpecificFailureMechanisms.Contains(fp)))
            {
                throw new ArgumentException($"{nameof(AssessmentSectionMergeData.MergeSpecificFailureMechanisms)} must contain items of " +
                                            $"the assessment section in {nameof(mergeData)}.");
            }
        }

        private void AfterMerge(IEnumerable<IObservable> changedObjects)
        {
            documentViewController.CloseAllViews();
            changedObjects.ForEachElementDo(co => co.NotifyObservers());
        }

        private static void LogMergeMessage(SpecificFailureMechanism failureMechanism)
        {
            log.InfoFormat(Resources.AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_added, failureMechanism.Name);
        }

        private static void LogMergeMessage(IFailureMechanism failureMechanism)
        {
            log.InfoFormat(Resources.AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replaced, failureMechanism.Name);
        }

        #region SpecificFailureMechanisms

        private static void MergeSpecificFailureMechanism(AssessmentSection targetAssessmentSection, IEnumerable<SpecificFailureMechanism> mergeFailureMechanisms)
        {
            if (mergeFailureMechanisms.Any())
            {
                targetAssessmentSection.SpecificFailureMechanisms.AddRange(mergeFailureMechanisms);
                mergeFailureMechanisms.ForEachElementDo(LogMergeMessage);
            }
        }

        #endregion

        #region HydraulicBoundaryData

        private static IEnumerable<IObservable> MergeHydraulicBoundaryData(
            AssessmentSection targetAssessmentSection, AssessmentSection sourceAssessmentSection,
            IHydraulicBoundaryDataUpdateHandler hydraulicBoundaryDataUpdateHandler)
        {
            var changedObjects = new List<IObservable>();

            changedObjects.AddRange(MergeHydraulicBoundaryDatabases(targetAssessmentSection, sourceAssessmentSection, hydraulicBoundaryDataUpdateHandler));

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(targetAssessmentSection, sourceAssessmentSection));
            changedObjects.AddRange(MergeDuneLocationCalculationsForUserDefinedTargetProbabilities(targetAssessmentSection, sourceAssessmentSection));

            foreach (HydraulicBoundaryDatabase sourceHydraulicBoundaryDatabase in sourceAssessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases)
            {
                HydraulicBoundaryDatabase targetHydraulicBoundaryDatabase =
                    targetAssessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases
                                           .First(hbd => Path.GetFileNameWithoutExtension(hbd.FilePath) == Path.GetFileNameWithoutExtension(sourceHydraulicBoundaryDatabase.FilePath));

                changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationData(targetAssessmentSection,
                                                                                      sourceAssessmentSection,
                                                                                      targetHydraulicBoundaryDatabase.Locations,
                                                                                      sourceHydraulicBoundaryDatabase.Locations));

                changedObjects.AddRange(MergeDuneLocationCalculationData(
                                            targetAssessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities,
                                            sourceAssessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities,
                                            targetHydraulicBoundaryDatabase.Locations,
                                            sourceHydraulicBoundaryDatabase.Locations));
            }

            log.Info(changedObjects.Any()
                         ? Resources.AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_merged
                         : Resources.AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_not_merged);

            return changedObjects;
        }

        private static IEnumerable<IObservable> MergeHydraulicBoundaryDatabases(
            IAssessmentSection targetAssessmentSection, IAssessmentSection sourceAssessmentSection,
            IHydraulicBoundaryDataUpdateHandler hydraulicBoundaryDataUpdateHandler)
        {
            var changedObjects = new List<IObservable>();

            IEnumerable<string> existingHydraulicBoundaryDatabaseFileNames =
                targetAssessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases
                                       .Select(hbd => Path.GetFileNameWithoutExtension(hbd.FilePath));

            IEnumerable<HydraulicBoundaryDatabase> hydraulicBoundaryDatabasesToAdd =
                sourceAssessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases
                                       .Where(hbd => !existingHydraulicBoundaryDatabaseFileNames.Contains(
                                                         Path.GetFileNameWithoutExtension(hbd.FilePath)));

            hydraulicBoundaryDatabasesToAdd.ForEachElementDo(
                hbd => changedObjects.AddRange(hydraulicBoundaryDataUpdateHandler.AddHydraulicBoundaryDatabase(hbd)));

            return changedObjects;
        }

        #region HydraulicBoundaryLocationCalculations

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(
            IAssessmentSection targetAssessmentSection,
            IAssessmentSection sourceAssessmentSection)
        {
            var changedObjects = new List<IObservable>();

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(
                                        targetAssessmentSection,
                                        targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                        sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities));

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(
                                        targetAssessmentSection,
                                        targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                        sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities));

            return changedObjects;
        }

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(
            IAssessmentSection targetAssessmentSection,
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> targetCalculationsForTargetProbabilities,
            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> sourceCalculationsForTargetProbabilities)
        {
            IEnumerable<double> existingTargetProbabilities = targetCalculationsForTargetProbabilities.Select(tc => tc.TargetProbability);

            IEnumerable<double> targetProbabilitiesToAdd = sourceCalculationsForTargetProbabilities.Select(sc => sc.TargetProbability)
                                                                                                   .Except(existingTargetProbabilities)
                                                                                                   .ToArray();

            if (targetProbabilitiesToAdd.Any())
            {
                targetProbabilitiesToAdd.ForEachElementDo(
                    tp => targetCalculationsForTargetProbabilities.Add(
                        HydraulicBoundaryLocationCalculationsForTargetProbabilityHelper.Create(targetAssessmentSection, tp)));

                yield return targetCalculationsForTargetProbabilities;
            }
        }

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocationCalculationData(IAssessmentSection targetAssessmentSection,
                                                                                              IAssessmentSection sourceAssessmentSection,
                                                                                              IEnumerable<HydraulicBoundaryLocation> targetHydraulicBoundaryLocations,
                                                                                              IEnumerable<HydraulicBoundaryLocation> sourceHydraulicBoundaryLocations)
        {
            var changedObjects = new List<IObservable>();

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationData(
                                        targetAssessmentSection.WaterLevelCalculationsForSignalFloodingProbability
                                                               .Where(calculation => targetHydraulicBoundaryLocations.Contains(calculation.HydraulicBoundaryLocation)),
                                        sourceAssessmentSection.WaterLevelCalculationsForSignalFloodingProbability
                                                               .Where(calculation => sourceHydraulicBoundaryLocations.Contains(calculation.HydraulicBoundaryLocation))));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationData(
                                        targetAssessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability
                                                               .Where(calculation => targetHydraulicBoundaryLocations.Contains(calculation.HydraulicBoundaryLocation)),
                                        sourceAssessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability
                                                               .Where(calculation => sourceHydraulicBoundaryLocations.Contains(calculation.HydraulicBoundaryLocation))));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationData(
                                        targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                        sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                        targetHydraulicBoundaryLocations,
                                        sourceHydraulicBoundaryLocations));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationData(
                                        targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                        sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                        targetHydraulicBoundaryLocations,
                                        sourceHydraulicBoundaryLocations));

            return changedObjects;
        }

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocationCalculationData(IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations,
                                                                                              IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations)
        {
            for (var i = 0; i < targetCalculations.Count(); i++)
            {
                HydraulicBoundaryLocationCalculation targetCalculation = targetCalculations.ElementAt(i);
                HydraulicBoundaryLocationCalculation sourceCalculation = sourceCalculations.ElementAt(i);

                if (ShouldMerge(targetCalculation, sourceCalculation))
                {
                    MergeCalculationData(targetCalculation, sourceCalculation);

                    yield return targetCalculation;
                }
            }
        }

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocationCalculationData(
            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> targetCalculations,
            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> sourceCalculations,
            IEnumerable<HydraulicBoundaryLocation> targetHydraulicBoundaryLocations,
            IEnumerable<HydraulicBoundaryLocation> sourceHydraulicBoundaryLocations)
        {
            var changedObjects = new List<IObservable>();

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability sourceCalculationsForTargetProbability in sourceCalculations)
            {
                HydraulicBoundaryLocationCalculationsForTargetProbability targetCalculationsForTargetProbability = targetCalculations.First(
                    c => c.TargetProbability.Equals(sourceCalculationsForTargetProbability.TargetProbability));

                changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculationData(
                                            targetCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations
                                                                                  .Where(c => targetHydraulicBoundaryLocations.Contains(c.HydraulicBoundaryLocation)),
                                            sourceCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations
                                                                                  .Where(c => sourceHydraulicBoundaryLocations.Contains(c.HydraulicBoundaryLocation))));
            }

            return changedObjects;
        }

        private static bool ShouldMerge(HydraulicBoundaryLocationCalculation targetCalculation, HydraulicBoundaryLocationCalculation sourceCalculation)
        {
            bool targetCalculationHasOutput = targetCalculation.HasOutput;
            bool sourceCalculationHasOutput = sourceCalculation.HasOutput;

            if (!targetCalculationHasOutput && !sourceCalculationHasOutput
                || targetCalculationHasOutput && !sourceCalculationHasOutput
                || targetCalculationHasOutput && !targetCalculation.Output.HasGeneralResult && !sourceCalculation.Output.HasGeneralResult
                || targetCalculationHasOutput && targetCalculation.Output.HasGeneralResult && sourceCalculation.Output.HasGeneralResult
                || targetCalculationHasOutput && targetCalculation.Output.HasGeneralResult && !sourceCalculation.Output.HasGeneralResult)
            {
                return false;
            }

            return true;
        }

        private static void MergeCalculationData(HydraulicBoundaryLocationCalculation targetCalculation, HydraulicBoundaryLocationCalculation sourceCalculation)
        {
            targetCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = sourceCalculation.InputParameters.ShouldIllustrationPointsBeCalculated;
            targetCalculation.Output = sourceCalculation.Output;
        }

        #endregion

        #region DuneLocationCalculations

        private static IEnumerable<IObservable> MergeDuneLocationCalculationsForUserDefinedTargetProbabilities(AssessmentSection targetAssessmentSection,
                                                                                                               AssessmentSection sourceAssessmentSection)
        {
            ObservableList<DuneLocationCalculationsForTargetProbability> targetCalculationsForTargetProbabilities =
                targetAssessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities;

            IEnumerable<double> existingTargetProbabilities = targetCalculationsForTargetProbabilities.Select(tc => tc.TargetProbability);

            IEnumerable<double> targetProbabilitiesToAdd = sourceAssessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities
                                                                                  .Select(sc => sc.TargetProbability)
                                                                                  .Except(existingTargetProbabilities)
                                                                                  .ToArray();

            if (targetProbabilitiesToAdd.Any())
            {
                targetProbabilitiesToAdd.ForEachElementDo(
                    tp => targetCalculationsForTargetProbabilities.Add(
                        CreateDuneLocationCalculationsForTargetProbability(targetAssessmentSection.DuneErosion, tp)));

                yield return targetCalculationsForTargetProbabilities;
            }
        }

        private static DuneLocationCalculationsForTargetProbability CreateDuneLocationCalculationsForTargetProbability(DuneErosionFailureMechanism failureMechanism,
                                                                                                                       double targetProbability)
        {
            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(targetProbability);

            duneLocationCalculationsForTargetProbability.DuneLocationCalculations.AddRange(
                failureMechanism.DuneLocations.Select(dl => new DuneLocationCalculation(dl)));

            return duneLocationCalculationsForTargetProbability;
        }

        private static IEnumerable<IObservable> MergeDuneLocationCalculationData(
            IEnumerable<DuneLocationCalculationsForTargetProbability> targetCalculations,
            IEnumerable<DuneLocationCalculationsForTargetProbability> sourceCalculations,
            IEnumerable<HydraulicBoundaryLocation> targetHydraulicBoundaryLocations,
            IEnumerable<HydraulicBoundaryLocation> sourceHydraulicBoundaryLocations)
        {
            var changedObjects = new List<IObservable>();

            foreach (DuneLocationCalculationsForTargetProbability sourceCalculationsForTargetProbability in sourceCalculations)
            {
                DuneLocationCalculationsForTargetProbability targetCalculationsForTargetProbability = targetCalculations.First(
                    c => c.TargetProbability.Equals(sourceCalculationsForTargetProbability.TargetProbability));

                changedObjects.AddRange(MergeDuneLocationCalculationData(
                                            targetCalculationsForTargetProbability.DuneLocationCalculations
                                                                                  .Where(c => targetHydraulicBoundaryLocations.Contains(c.DuneLocation.HydraulicBoundaryLocation)),
                                            sourceCalculationsForTargetProbability.DuneLocationCalculations
                                                                                  .Where(c => sourceHydraulicBoundaryLocations.Contains(c.DuneLocation.HydraulicBoundaryLocation))));
            }

            return changedObjects;
        }

        private static IEnumerable<IObservable> MergeDuneLocationCalculationData(IEnumerable<DuneLocationCalculation> targetCalculations,
                                                                                 IEnumerable<DuneLocationCalculation> sourceCalculations)
        {
            for (var i = 0; i < targetCalculations.Count(); i++)
            {
                DuneLocationCalculation targetCalculation = targetCalculations.ElementAt(i);
                DuneLocationCalculation sourceCalculation = sourceCalculations.ElementAt(i);

                if (ShouldMerge(targetCalculation, sourceCalculation))
                {
                    MergeCalculationData(targetCalculation, sourceCalculation);

                    yield return targetCalculation;
                }
            }
        }

        private static bool ShouldMerge(DuneLocationCalculation targetCalculation, DuneLocationCalculation sourceCalculation)
        {
            return targetCalculation.Output == null && sourceCalculation.Output != null;
        }

        private static void MergeCalculationData(DuneLocationCalculation targetCalculation, DuneLocationCalculation sourceCalculation)
        {
            targetCalculation.Output = sourceCalculation.Output;
        }

        #endregion

        #endregion

        #region FailureMechanisms

        private static void MergeFailureMechanisms(AssessmentSection targetAssessmentSection, AssessmentSectionMergeData mergeData)
        {
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = targetAssessmentSection.HydraulicBoundaryData.GetLocations();
            AssessmentSection sourceAssessmentSection = mergeData.AssessmentSection;

            if (mergeData.MergePiping)
            {
                targetAssessmentSection.Piping = sourceAssessmentSection.Piping;
                UpdateCalculationHydraulicBoundaryLocationReferences<PipingFailureMechanism, IPipingCalculationScenario<PipingInput>, PipingInput>(
                    targetAssessmentSection.Piping, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.Piping);
            }

            if (mergeData.MergeGrassCoverErosionInwards)
            {
                targetAssessmentSection.GrassCoverErosionInwards = sourceAssessmentSection.GrassCoverErosionInwards;
                UpdateCalculationHydraulicBoundaryLocationReferences<GrassCoverErosionInwardsFailureMechanism, GrassCoverErosionInwardsCalculationScenario, GrassCoverErosionInwardsInput>(
                    targetAssessmentSection.GrassCoverErosionInwards, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.GrassCoverErosionInwards);
            }

            if (mergeData.MergeMacroStabilityInwards)
            {
                targetAssessmentSection.MacroStabilityInwards = sourceAssessmentSection.MacroStabilityInwards;
                UpdateCalculationHydraulicBoundaryLocationReferences<MacroStabilityInwardsFailureMechanism, MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput>(
                    targetAssessmentSection.MacroStabilityInwards, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.MacroStabilityInwards);
            }

            if (mergeData.MergeMicrostability)
            {
                targetAssessmentSection.Microstability = sourceAssessmentSection.Microstability;
                LogMergeMessage(targetAssessmentSection.Microstability);
            }

            if (mergeData.MergeStabilityStoneCover)
            {
                targetAssessmentSection.StabilityStoneCover = sourceAssessmentSection.StabilityStoneCover;
                UpdateCalculationHydraulicBoundaryLocationReferences<StabilityStoneCoverFailureMechanism, StabilityStoneCoverWaveConditionsCalculation, StabilityStoneCoverWaveConditionsInput>(
                    targetAssessmentSection.StabilityStoneCover, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.StabilityStoneCover);
            }

            if (mergeData.MergeWaveImpactAsphaltCover)
            {
                targetAssessmentSection.WaveImpactAsphaltCover = sourceAssessmentSection.WaveImpactAsphaltCover;
                UpdateCalculationHydraulicBoundaryLocationReferences<WaveImpactAsphaltCoverFailureMechanism, WaveImpactAsphaltCoverWaveConditionsCalculation, WaveConditionsInput>(
                    targetAssessmentSection.WaveImpactAsphaltCover, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.WaveImpactAsphaltCover);
            }

            if (mergeData.MergeWaterPressureAsphaltCover)
            {
                targetAssessmentSection.WaterPressureAsphaltCover = sourceAssessmentSection.WaterPressureAsphaltCover;
                LogMergeMessage(targetAssessmentSection.WaterPressureAsphaltCover);
            }

            if (mergeData.MergeGrassCoverErosionOutwards)
            {
                targetAssessmentSection.GrassCoverErosionOutwards = sourceAssessmentSection.GrassCoverErosionOutwards;
                UpdateCalculationHydraulicBoundaryLocationReferences<GrassCoverErosionOutwardsFailureMechanism, GrassCoverErosionOutwardsWaveConditionsCalculation, GrassCoverErosionOutwardsWaveConditionsInput>(
                    targetAssessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.GrassCoverErosionOutwards);
            }

            if (mergeData.MergeGrassCoverSlipOffOutwards)
            {
                targetAssessmentSection.GrassCoverSlipOffOutwards = sourceAssessmentSection.GrassCoverSlipOffOutwards;
                LogMergeMessage(targetAssessmentSection.GrassCoverSlipOffOutwards);
            }

            if (mergeData.MergeGrassCoverSlipOffInwards)
            {
                targetAssessmentSection.GrassCoverSlipOffInwards = sourceAssessmentSection.GrassCoverSlipOffInwards;
                LogMergeMessage(targetAssessmentSection.GrassCoverSlipOffInwards);
            }

            if (mergeData.MergeHeightStructures)
            {
                targetAssessmentSection.HeightStructures = sourceAssessmentSection.HeightStructures;
                UpdateCalculationHydraulicBoundaryLocationReferences<HeightStructuresFailureMechanism, StructuresCalculationScenario<HeightStructuresInput>, HeightStructuresInput>(
                    targetAssessmentSection.HeightStructures, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.HeightStructures);
            }

            if (mergeData.MergeClosingStructures)
            {
                targetAssessmentSection.ClosingStructures = sourceAssessmentSection.ClosingStructures;
                UpdateCalculationHydraulicBoundaryLocationReferences<ClosingStructuresFailureMechanism, StructuresCalculationScenario<ClosingStructuresInput>, ClosingStructuresInput>(
                    targetAssessmentSection.ClosingStructures, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.ClosingStructures);
            }

            if (mergeData.MergePipingStructure)
            {
                targetAssessmentSection.PipingStructure = sourceAssessmentSection.PipingStructure;
                LogMergeMessage(targetAssessmentSection.PipingStructure);
            }

            if (mergeData.MergeStabilityPointStructures)
            {
                targetAssessmentSection.StabilityPointStructures = sourceAssessmentSection.StabilityPointStructures;
                UpdateCalculationHydraulicBoundaryLocationReferences<StabilityPointStructuresFailureMechanism, StructuresCalculationScenario<StabilityPointStructuresInput>, StabilityPointStructuresInput>(
                    targetAssessmentSection.StabilityPointStructures, hydraulicBoundaryLocations);
                LogMergeMessage(targetAssessmentSection.StabilityPointStructures);
            }

            if (mergeData.MergeDuneErosion)
            {
                targetAssessmentSection.DuneErosion = sourceAssessmentSection.DuneErosion;
                LogMergeMessage(targetAssessmentSection.DuneErosion);
            }
        }

        private static void UpdateCalculationHydraulicBoundaryLocationReferences<TFailureMechanism, TCalculation, TCalculationInput>(
            TFailureMechanism failureMechanism, IEnumerable<HydraulicBoundaryLocation> locations)
            where TFailureMechanism : ICalculatableFailureMechanism
            where TCalculation : ICalculation<TCalculationInput>
            where TCalculationInput : class, ICalculationInputWithHydraulicBoundaryLocation
        {
            foreach (TCalculation calculation in failureMechanism.Calculations.Cast<TCalculation>())
            {
                if (calculation.InputParameters.HydraulicBoundaryLocation != null)
                {
                    calculation.InputParameters.HydraulicBoundaryLocation = GetHydraulicBoundaryLocation(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                         locations);
                }
            }
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocation(HydraulicBoundaryLocation location, IEnumerable<HydraulicBoundaryLocation> locations)
        {
            return locations.Single(l => l.Name == location.Name && l.Id == location.Id && l.Location.Equals(location.Location));
        }

        #endregion
    }
}