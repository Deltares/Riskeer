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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Assembly
{
    /// <summary>
    /// Class containing helper methods for <see cref="AssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionHelper
    {
        /// <summary>
        /// Determines if the assessment section has section assembly results that are manually overwritten.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/>.</param>
        /// <returns><c>true</c> if the assessment section contains section assembly results that are manually overwritten,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static bool HasManualAssemblyResults(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            if (pipingFailureMechanism.IsRelevant)
            {
                return PipingFailureMechanismHelper.HasManualAssemblyResults(pipingFailureMechanism);
            }

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            if (grassCoverErosionInwardsFailureMechanism.IsRelevant)
            {
                return GrassCoverErosionInwardsFailureMechanismHelper.HasManualAssemblyResults(grassCoverErosionInwardsFailureMechanism);
            }

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            if (macroStabilityInwardsFailureMechanism.IsRelevant)
            {
                return MacroStabilityInwardsFailureMechanismHelper.HasManualAssemblyResults(macroStabilityInwardsFailureMechanism);
            }

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            if (macroStabilityOutwardsFailureMechanism.IsRelevant)
            {
                return MacroStabilityOutwardsFailureMechanismHelper.HasManualAssemblyResults(macroStabilityOutwardsFailureMechanism);
            }

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            if (microstabilityFailureMechanism.IsRelevant)
            {
                return MicrostabilityFailureMechanismHelper.HasManualAssemblyResults(microstabilityFailureMechanism);
            }

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            if (stabilityStoneCoverFailureMechanism.IsRelevant)
            {
                return StabilityStoneCoverFailureMechanismHelper.HasManualAssemblyResults(stabilityStoneCoverFailureMechanism);
            }

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            if (waveImpactAsphaltCoverFailureMechanism.IsRelevant)
            {
                return WaveImpactAsphaltCoverFailureMechanismHelper.HasManualAssemblyResults(waveImpactAsphaltCoverFailureMechanism);
            }

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            if (waterPressureAsphaltCoverFailureMechanism.IsRelevant)
            {
                return WaterPressureAsphaltCoverFailureMechanismHelper.HasManualAssemblyResults(waterPressureAsphaltCoverFailureMechanism);
            }

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            if (grassCoverErosionOutwardsFailureMechanism.IsRelevant)
            {
                return GrassCoverErosionOutwardsFailureMechanismHelper.HasManualAssemblyResults(grassCoverErosionOutwardsFailureMechanism);
            }

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            if (grassCoverSlipOffOutwardsFailureMechanism.IsRelevant)
            {
                return GrassCoverSlipOffOutwardsFailureMechanismHelper.HasManualAssemblyResults(grassCoverSlipOffOutwardsFailureMechanism);
            }

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            if (grassCoverSlipOffInwardsFailureMechanism.IsRelevant)
            {
                return GrassCoverSlipOffInwardsFailureMechanismHelper.HasManualAssemblyResults(grassCoverSlipOffInwardsFailureMechanism);
            }

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            if (heightStructuresFailureMechanism.IsRelevant)
            {
                return HeightStructuresFailureMechanismHelper.HasManualAssemblyResults(heightStructuresFailureMechanism);
            }

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            if (closingStructuresFailureMechanism.IsRelevant)
            {
                return ClosingStructuresFailureMechanismHelper.HasManualAssemblyResults(closingStructuresFailureMechanism);
            }

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            if (pipingStructureFailureMechanism.IsRelevant)
            {
                return PipingStructureFailureMechanismHelper.HasManualAssemblyResults(pipingStructureFailureMechanism);
            }

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            if (stabilityPointStructuresFailureMechanism.IsRelevant)
            {
                return StabilityPointStructuresFailureMechanismHelper.HasManualAssemblyResults(stabilityPointStructuresFailureMechanism);
            }

            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            if (strengthStabilityLengthwiseConstructionFailureMechanism.IsRelevant)
            {
                return StrengthStabilityLengthwiseConstructionFailureMechanismHelper.HasManualAssemblyResults(strengthStabilityLengthwiseConstructionFailureMechanism);
            }

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            if (duneErosionFailureMechanism.IsRelevant)
            {
                return DuneErosionFailureMechanismHelper.HasManualAssemblyResults(duneErosionFailureMechanism);
            }

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            if (technicalInnovationFailureMechanism.IsRelevant)
            {
                return TechnicalInnovationFailureMechanismHelper.HasManualAssemblyResults(technicalInnovationFailureMechanism);
            }

            return false;
        }
    }
}