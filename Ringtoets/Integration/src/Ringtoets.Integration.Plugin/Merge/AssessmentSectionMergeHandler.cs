// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.Commands;
using Core.Common.Util.Extensions;
using log4net;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Plugin.Merge
{
    /// <summary>
    /// Class responsible for handling the merge of <see cref="AssessmentSection"/> data.
    /// </summary>
    public class AssessmentSectionMergeHandler : IAssessmentSectionMergeHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionMergeHandler));

        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeHandler"/>.
        /// </summary>
        /// <param name="viewCommands">The view commands used to close views for the target
        /// <see cref="AssessmentSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewCommands"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionMergeHandler(IViewCommands viewCommands)
        {
            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.viewCommands = viewCommands;
        }

        public void PerformMerge(AssessmentSection targetAssessmentSection, AssessmentSection sourceAssessmentSection,
                                 IEnumerable<IFailureMechanism> failureMechanismsToMerge)
        {
            if (targetAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(targetAssessmentSection));
            }

            if (sourceAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(sourceAssessmentSection));
            }

            if (failureMechanismsToMerge == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismsToMerge));
            }

            BeforeMerge(targetAssessmentSection);

            var changedObjects = new List<IObservable>
            {
                targetAssessmentSection
            };

            changedObjects.AddRange(MergeHydraulicBoundaryLocations(targetAssessmentSection, sourceAssessmentSection));
            MergeFailureMechanisms(targetAssessmentSection, failureMechanismsToMerge);

            AfterMerge(changedObjects);
        }

        private void BeforeMerge(AssessmentSection assessmentSection)
        {
            viewCommands.RemoveAllViewsForItem(assessmentSection);
        }

        private static void AfterMerge(IEnumerable<IObservable> changedObjects)
        {
            changedObjects.ForEachElementDo(co => co.NotifyObservers());
        }

        #region HydraulicBoundaryLocationCalculations

        private static IEnumerable<IObservable> MergeHydraulicBoundaryLocations(IAssessmentSection targetAssessmentSection, IAssessmentSection sourceAssessmentSection)
        {
            var changedObjects = new List<IObservable>();

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm,
                                                                               sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                                               sourceAssessmentSection.WaterLevelCalculationsForSignalingNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                                               sourceAssessmentSection.WaterLevelCalculationsForLowerLimitNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                                                                               sourceAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm));

            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                                                                               sourceAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaveHeightCalculationsForSignalingNorm,
                                                                               sourceAssessmentSection.WaveHeightCalculationsForSignalingNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                                                                               sourceAssessmentSection.WaveHeightCalculationsForLowerLimitNorm));
            changedObjects.AddRange(MergeHydraulicBoundaryLocationCalculations(targetAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                                                                               sourceAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm));

            log.Info(Resources.AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_merged);

            return changedObjects;
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

        private static void MergeFailureMechanisms(AssessmentSection targetAssessmentSection, IEnumerable<IFailureMechanism> failureMechanismsToMerge)
        {
            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = targetAssessmentSection.HydraulicBoundaryDatabase.Locations;
            foreach (IFailureMechanism failureMechanism in failureMechanismsToMerge)
            {
                if (TryMergeFailureMechanism<PipingFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.Piping = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<PipingFailureMechanism, PipingCalculationScenario, PipingInput>(
                        targetAssessmentSection.Piping, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<GrassCoverErosionInwardsFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.GrassCoverErosionInwards = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<GrassCoverErosionInwardsFailureMechanism, GrassCoverErosionInwardsCalculation, GrassCoverErosionInwardsInput>(
                        targetAssessmentSection.GrassCoverErosionInwards, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<MacroStabilityInwardsFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.MacroStabilityInwards = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<MacroStabilityInwardsFailureMechanism, MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput>(
                        targetAssessmentSection.MacroStabilityInwards, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<MacroStabilityOutwardsFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.MacroStabilityOutwards = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<MicrostabilityFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.Microstability = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<StabilityStoneCoverFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.StabilityStoneCover = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<StabilityStoneCoverFailureMechanism, StabilityStoneCoverWaveConditionsCalculation, AssessmentSectionCategoryWaveConditionsInput>(
                        targetAssessmentSection.StabilityStoneCover, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<WaveImpactAsphaltCoverFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.WaveImpactAsphaltCover = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<WaveImpactAsphaltCoverFailureMechanism, WaveImpactAsphaltCoverWaveConditionsCalculation, AssessmentSectionCategoryWaveConditionsInput>(
                        targetAssessmentSection.WaveImpactAsphaltCover, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<WaterPressureAsphaltCoverFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.WaterPressureAsphaltCover = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<GrassCoverErosionOutwardsFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.GrassCoverErosionOutwards = mechanism))
                {
                    UpdateLocationCalculationHydraulicBoundaryLocationReferences(targetAssessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocations);
                    UpdateCalculationHydraulicBoundaryLocationReferences<GrassCoverErosionOutwardsFailureMechanism, GrassCoverErosionOutwardsWaveConditionsCalculation, FailureMechanismCategoryWaveConditionsInput>(
                        targetAssessmentSection.GrassCoverErosionOutwards, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<GrassCoverSlipOffOutwardsFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.GrassCoverSlipOffOutwards = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<GrassCoverSlipOffInwardsFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.GrassCoverSlipOffInwards = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<HeightStructuresFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.HeightStructures = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<HeightStructuresFailureMechanism, StructuresCalculation<HeightStructuresInput>, HeightStructuresInput>(
                        targetAssessmentSection.HeightStructures, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<ClosingStructuresFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.ClosingStructures = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<ClosingStructuresFailureMechanism, StructuresCalculation<ClosingStructuresInput>, ClosingStructuresInput>(
                        targetAssessmentSection.ClosingStructures, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<PipingStructureFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.PipingStructure = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<StabilityPointStructuresFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.StabilityPointStructures = mechanism))
                {
                    UpdateCalculationHydraulicBoundaryLocationReferences<StabilityPointStructuresFailureMechanism, StructuresCalculation<StabilityPointStructuresInput>, StabilityPointStructuresInput>(
                        targetAssessmentSection.StabilityPointStructures, hydraulicBoundaryLocations);
                    continue;
                }

                if (TryMergeFailureMechanism<StrengthStabilityLengthwiseConstructionFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.StrengthStabilityLengthwiseConstruction = mechanism))
                {
                    continue;
                }

                if (TryMergeFailureMechanism<DuneErosionFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.DuneErosion = mechanism))
                {
                    continue;
                }

                TryMergeFailureMechanism<TechnicalInnovationFailureMechanism>(
                    targetAssessmentSection, failureMechanism,
                    (section, mechanism) => section.TechnicalInnovation = mechanism);
            }
        }

        private static bool TryMergeFailureMechanism<TFailureMechanism>(AssessmentSection targetAssessmentSection, IFailureMechanism failureMechanismToMerge,
                                                                        Action<AssessmentSection, TFailureMechanism> mergeFailureMechanismAction)
            where TFailureMechanism : class, IFailureMechanism
        {
            var failureMechanism = failureMechanismToMerge as TFailureMechanism;
            if (failureMechanism != null)
            {
                mergeFailureMechanismAction(targetAssessmentSection, failureMechanism);
                log.InfoFormat(Resources.AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replaced, failureMechanism.Name);
                return true;
            }

            return false;
        }

        private static void UpdateCalculationHydraulicBoundaryLocationReferences<TFailureMechanism, TCalculation, TCalculationInput>(
            TFailureMechanism failureMechanism, IEnumerable<HydraulicBoundaryLocation> locations)
            where TFailureMechanism : IFailureMechanism
            where TCalculation : ICalculation<TCalculationInput>
            where TCalculationInput : class, ICalculationInputWithLocation
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

            ReplaceCalculationData(oldWaterLevelForMechanismSpecificFactorizedSignalingNorm, targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            ReplaceCalculationData(oldWaterLevelForMechanismSpecificSignalingNorm, targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            ReplaceCalculationData(oldWaterLevelForMechanismSpecificLowerLimitNorm, targetFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);

            ReplaceCalculationData(oldWaveHeightForMechanismSpecificFactorizedSignalingNorm, targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            ReplaceCalculationData(oldWaveHeightForMechanismSpecificSignalingNorm, targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            ReplaceCalculationData(oldWaveHeightForMechanismSpecificLowerLimitNorm, targetFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        private static void ReplaceCalculationData(IEnumerable<HydraulicBoundaryLocationCalculation> oldCalculations, IEnumerable<HydraulicBoundaryLocationCalculation> newCalculations)
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