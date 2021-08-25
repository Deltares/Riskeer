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
using Core.Common.Util.Extensions;
using log4net;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
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

        public void PerformMerge(AssessmentSection targetAssessmentSection, AssessmentSectionMergeData mergeData)
        {
            if (targetAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(targetAssessmentSection));
            }

            if (mergeData == null)
            {
                throw new ArgumentNullException(nameof(mergeData));
            }

            var changedObjects = new List<IObservable>
            {
                targetAssessmentSection
            };

            changedObjects.AddRange(MergeHydraulicBoundaryLocations(targetAssessmentSection, mergeData.AssessmentSection));
            MergeFailureMechanisms(targetAssessmentSection, mergeData);

            AfterMerge(changedObjects);
        }

        private static void AfterMerge(IEnumerable<IObservable> changedObjects)
        {
            changedObjects.ForEachElementDo(co => co.NotifyObservers());
        }

        #region HydraulicBoundaryLocationCalculations

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocations(IAssessmentSection targetAssessmentSection, IAssessmentSection sourceAssessmentSection)
        {
            var changedObjects = new List<IObservable>();

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                                               sourceAssessmentSection.WaterLevelCalculationsForSignalingNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                                               sourceAssessmentSection.WaterLevelCalculationsForLowerLimitNorm));

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> sourceWaterLevelProbabilities = sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities;

            HydraulicBoundaryLocationCalculationsForTargetProbability[] waterLevelProbabilitiesToMerge = sourceAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                                                .Where(sp => targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                                                                                    .Select(c => c.TargetProbability)
                                                                                                                                                                    .Contains(sp.TargetProbability)).ToArray();

            MergeWaterLevelProbabilities(targetAssessmentSection, changedObjects, waterLevelProbabilitiesToMerge);

            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> uniqueWaterLevelSourceProbabilities = sourceWaterLevelProbabilities.Except(waterLevelProbabilitiesToMerge);

            MergeUniqueWaterLevelProbabilities(targetAssessmentSection, uniqueWaterLevelSourceProbabilities);

            ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> sourceWaveHeightProbabilities = sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities;

            HydraulicBoundaryLocationCalculationsForTargetProbability[] waveHeightProbabilitiesToMerge = sourceAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                                                                                .Where(sp => targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                                                                                                                    .Select(c => c.TargetProbability)
                                                                                                                                                                    .Contains(sp.TargetProbability)).ToArray();

            MergeWaveHeightProbabilities(targetAssessmentSection, changedObjects, waveHeightProbabilitiesToMerge);

            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> uniqueWaveHeightSourceProbabilities = sourceWaveHeightProbabilities.Except(waveHeightProbabilitiesToMerge);

            MergeUniqueWaveHeightProbabilities(targetAssessmentSection, uniqueWaveHeightSourceProbabilities);

            log.Info(changedObjects.Any()
                         ? Resources.AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_merged
                         : Resources.AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_not_merged);

            return changedObjects;
        }

        private static void MergeWaterLevelProbabilities(IAssessmentSection targetAssessmentSection, List<IObservable> changedObjects, HydraulicBoundaryLocationCalculationsForTargetProbability[] probabilitiesToMerge)
        {
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability sourceProbability in probabilitiesToMerge)
            {
                HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability = targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First(c => c.TargetProbability.Equals(sourceProbability.TargetProbability));

                changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetProbability.HydraulicBoundaryLocationCalculations, sourceProbability.HydraulicBoundaryLocationCalculations));
            }
        }

        private static void MergeUniqueWaterLevelProbabilities(IAssessmentSection targetAssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities)
        {
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability in targetProbabilities)
            {
                var newTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability
                {
                    TargetProbability = targetProbability.TargetProbability
                };

                newTargetProbability.HydraulicBoundaryLocationCalculations.AddRange(targetProbability.HydraulicBoundaryLocationCalculations
                                                                                                     .Select(calculation => new HydraulicBoundaryLocationCalculation(GetHydraulicBoundaryLocation(calculation.HydraulicBoundaryLocation, targetAssessmentSection.HydraulicBoundaryDatabase.Locations)))
                                                                                                     .ToArray());

                targetAssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Add(newTargetProbability);
            }
        }

        private static void MergeWaveHeightProbabilities(IAssessmentSection targetAssessmentSection, List<IObservable> changedObjects, HydraulicBoundaryLocationCalculationsForTargetProbability[] probabilitiesToMerge)
        {
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability sourceProbability in probabilitiesToMerge)
            {
                HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability = targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.First(c => c.TargetProbability.Equals(sourceProbability.TargetProbability));

                changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetProbability.HydraulicBoundaryLocationCalculations, sourceProbability.HydraulicBoundaryLocationCalculations));
            }
        }

        private static void MergeUniqueWaveHeightProbabilities(IAssessmentSection targetAssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> targetProbabilities)
        {
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability targetProbability in targetProbabilities)
            {
                var newTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability
                {
                    TargetProbability = targetProbability.TargetProbability
                };

                newTargetProbability.HydraulicBoundaryLocationCalculations.AddRange(targetProbability.HydraulicBoundaryLocationCalculations
                                                                                                     .Select(calculation => new HydraulicBoundaryLocationCalculation(GetHydraulicBoundaryLocation(calculation.HydraulicBoundaryLocation, targetAssessmentSection.HydraulicBoundaryDatabase.Locations)))
                                                                                                     .ToArray());

                targetAssessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Add(newTargetProbability);
            }
        }

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations,
                                                                                           IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations)
        {
            for (var i = 0; i < targetCalculations.Count(); i++)
            {
                HydraulicBoundaryLocationCalculation targetCalculation = targetCalculations.ElementAt(i);
                HydraulicBoundaryLocationCalculation sourceCalculation = sourceCalculations.ElementAt(i);

                if (ShouldMerge(targetCalculation, sourceCalculation))
                {
                    targetCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = sourceCalculation.InputParameters.ShouldIllustrationPointsBeCalculated;
                    targetCalculation.Output = sourceCalculation.Output;

                    yield return targetCalculation;
                }
            }
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

        #endregion

        #region FailureMechanisms

        private static void MergeFailureMechanisms(AssessmentSection targetAssessmentSection, AssessmentSectionMergeData mergeData)
        {
            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = targetAssessmentSection.HydraulicBoundaryDatabase.Locations;
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

            if (mergeData.MergeMacroStabilityOutwards)
            {
                targetAssessmentSection.MacroStabilityOutwards = sourceAssessmentSection.MacroStabilityOutwards;
                LogMergeMessage(targetAssessmentSection.MacroStabilityOutwards);
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
                UpdateCalculationHydraulicBoundaryLocationReferences<WaveImpactAsphaltCoverFailureMechanism, WaveImpactAsphaltCoverWaveConditionsCalculation, AssessmentSectionCategoryWaveConditionsInput>(
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
                UpdateLocationCalculationHydraulicBoundaryLocationReferences(targetAssessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocations);
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

            if (mergeData.MergeStrengthStabilityLengthwiseConstruction)
            {
                targetAssessmentSection.StrengthStabilityLengthwiseConstruction = sourceAssessmentSection.StrengthStabilityLengthwiseConstruction;
                LogMergeMessage(targetAssessmentSection.StrengthStabilityLengthwiseConstruction);
            }

            if (mergeData.MergeDuneErosion)
            {
                targetAssessmentSection.DuneErosion = sourceAssessmentSection.DuneErosion;
                LogMergeMessage(targetAssessmentSection.DuneErosion);
            }

            if (mergeData.MergeTechnicalInnovation)
            {
                targetAssessmentSection.TechnicalInnovation = sourceAssessmentSection.TechnicalInnovation;
                LogMergeMessage(targetAssessmentSection.TechnicalInnovation);
            }
        }

        private static void LogMergeMessage(IFailureMechanism failureMechanism)
        {
            log.InfoFormat(Resources.AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replaced, failureMechanism.Name);
        }

        private static void UpdateCalculationHydraulicBoundaryLocationReferences<TFailureMechanism, TCalculation, TCalculationInput>(
            TFailureMechanism failureMechanism, IEnumerable<HydraulicBoundaryLocation> locations)
            where TFailureMechanism : IFailureMechanism
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

        private static void UpdateLocationCalculationHydraulicBoundaryLocationReferences(GrassCoverErosionOutwardsFailureMechanism targetFailureMechanism,
                                                                                         IEnumerable<HydraulicBoundaryLocation> locations)
        {
            HydraulicBoundaryLocationCalculation[] oldWaterLevelForMechanismSpecificFactorizedSignalingNorm = targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ToArray();
            HydraulicBoundaryLocationCalculation[] oldWaterLevelForMechanismSpecificSignalingNorm = targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ToArray();
            HydraulicBoundaryLocationCalculation[] oldWaterLevelForMechanismSpecificLowerLimitNorm = targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ToArray();
            HydraulicBoundaryLocationCalculation[] oldWaveHeightForMechanismSpecificFactorizedSignalingNorm = targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.ToArray();
            HydraulicBoundaryLocationCalculation[] oldWaveHeightForMechanismSpecificSignalingNorm = targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.ToArray();
            HydraulicBoundaryLocationCalculation[] oldWaveHeightForMechanismSpecificLowerLimitNorm = targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.ToArray();
            targetFailureMechanism.SetHydraulicBoundaryLocationCalculations(locations);

            ReplaceHydraulicBoundaryLocationCalculationData(oldWaterLevelForMechanismSpecificFactorizedSignalingNorm, targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            ReplaceHydraulicBoundaryLocationCalculationData(oldWaterLevelForMechanismSpecificSignalingNorm, targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            ReplaceHydraulicBoundaryLocationCalculationData(oldWaterLevelForMechanismSpecificLowerLimitNorm, targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);

            ReplaceHydraulicBoundaryLocationCalculationData(oldWaveHeightForMechanismSpecificFactorizedSignalingNorm, targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            ReplaceHydraulicBoundaryLocationCalculationData(oldWaveHeightForMechanismSpecificSignalingNorm, targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            ReplaceHydraulicBoundaryLocationCalculationData(oldWaveHeightForMechanismSpecificLowerLimitNorm, targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        private static void ReplaceHydraulicBoundaryLocationCalculationData(IEnumerable<HydraulicBoundaryLocationCalculation> oldCalculations,
                                                                            IEnumerable<HydraulicBoundaryLocationCalculation> newCalculations)
        {
            for (var i = 0; i < newCalculations.Count(); i++)
            {
                HydraulicBoundaryLocationCalculation newCalculation = newCalculations.ElementAt(i);
                HydraulicBoundaryLocationCalculation oldCalculation = oldCalculations.ElementAt(i);

                newCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = oldCalculation.InputParameters.ShouldIllustrationPointsBeCalculated;
                newCalculation.Output = oldCalculation.Output;
            }
        }

        #endregion
    }
}