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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Observers;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Forms.Observers
{
    /// <summary>
    /// Class that observes all objects in an <see cref="AssessmentSection"/> related to
    /// its section results.
    /// </summary>
    public class AssessmentSectionResultObserver : Observable, IDisposable
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer referenceLineObserver;
        private readonly Observer closingStructuresObserver;
        private readonly Observer duneErosionObserver;
        private readonly Observer grassCoverErosionInwardsObserver;
        private readonly Observer grassCoverErosionOutwardsObserver;
        private readonly Observer heightStructuresObserver;
        private readonly Observer macroStabilityInwardsObserver;
        private readonly Observer pipingObserver;
        private readonly Observer stabilityPointStructuresObserver;
        private readonly Observer stabilityStoneCoverObserver;
        private readonly Observer waveImpactAsphaltCoverObserver;
        private readonly Observer grassCoverSlipOffInwardsObserver;
        private readonly Observer grassCoverSlipOffOutwardsObserver;
        private readonly Observer macroStabilityOutwardsObserver;
        private readonly Observer microstabilityObserver;
        private readonly Observer pipingStructureObserver;
        private readonly Observer strengthStabilityLengthwiseConstructionObserver;
        private readonly Observer technicalInnovationObserver;
        private readonly Observer waterPressureAsphaltCoverObserver;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionResultObserver"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to observe.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public AssessmentSectionResultObserver(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            assessmentSectionObserver = new Observer(() =>
            {
                ResubscribeFailureMechanismObservers(assessmentSection);
                NotifyObservers();
            })
            {
                Observable = assessmentSection
            };

            referenceLineObserver = new Observer(NotifyObservers)
            {
                Observable = assessmentSection.ReferenceLine
            };

            closingStructuresObserver = CreateCalculatableFailureMechanismObserver<ClosingStructuresFailureMechanism,
                ClosingStructuresFailureMechanismSectionResult, StructuresCalculation<ClosingStructuresInput>>(assessmentSection.ClosingStructures);

            duneErosionObserver = CreateFailureMechanismObserver<DuneErosionFailureMechanism,
                DuneErosionFailureMechanismSectionResult>(assessmentSection.DuneErosion);

            grassCoverErosionInwardsObserver = CreateCalculatableFailureMechanismObserver<GrassCoverErosionInwardsFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismSectionResult, GrassCoverErosionInwardsCalculation>(assessmentSection.GrassCoverErosionInwards);

            grassCoverErosionOutwardsObserver = CreateFailureMechanismObserver<GrassCoverErosionOutwardsFailureMechanism,
                GrassCoverErosionOutwardsFailureMechanismSectionResult>(assessmentSection.GrassCoverErosionOutwards);

            heightStructuresObserver = CreateCalculatableFailureMechanismObserver<HeightStructuresFailureMechanism,
                HeightStructuresFailureMechanismSectionResult, StructuresCalculation<HeightStructuresInput>>(assessmentSection.HeightStructures);

            macroStabilityInwardsObserver = CreateCalculatableFailureMechanismObserver<MacroStabilityInwardsFailureMechanism,
                MacroStabilityInwardsFailureMechanismSectionResult, MacroStabilityInwardsCalculationScenario>(assessmentSection.MacroStabilityInwards);

            pipingObserver = CreateCalculatableFailureMechanismObserver<PipingFailureMechanism,
                PipingFailureMechanismSectionResult, PipingCalculationScenario>(assessmentSection.Piping);

            stabilityPointStructuresObserver = CreateCalculatableFailureMechanismObserver<StabilityPointStructuresFailureMechanism,
                StabilityPointStructuresFailureMechanismSectionResult, StructuresCalculation<StabilityPointStructuresInput>>(assessmentSection.StabilityPointStructures);

            stabilityStoneCoverObserver = CreateFailureMechanismObserver<StabilityStoneCoverFailureMechanism,
                StabilityStoneCoverFailureMechanismSectionResult>(assessmentSection.StabilityStoneCover);

            waveImpactAsphaltCoverObserver = CreateFailureMechanismObserver<WaveImpactAsphaltCoverFailureMechanism,
                WaveImpactAsphaltCoverFailureMechanismSectionResult>(assessmentSection.WaveImpactAsphaltCover);

            grassCoverSlipOffInwardsObserver = CreateFailureMechanismObserver<GrassCoverSlipOffInwardsFailureMechanism,
                GrassCoverSlipOffInwardsFailureMechanismSectionResult>(assessmentSection.GrassCoverSlipOffInwards);

            grassCoverSlipOffOutwardsObserver = CreateFailureMechanismObserver<GrassCoverSlipOffOutwardsFailureMechanism,
                GrassCoverSlipOffOutwardsFailureMechanismSectionResult>(assessmentSection.GrassCoverSlipOffOutwards);

            macroStabilityOutwardsObserver = CreateFailureMechanismObserver<MacroStabilityOutwardsFailureMechanism,
                MacroStabilityOutwardsFailureMechanismSectionResult>(assessmentSection.MacroStabilityOutwards);

            microstabilityObserver = CreateFailureMechanismObserver<MicrostabilityFailureMechanism,
                MicrostabilityFailureMechanismSectionResult>(assessmentSection.Microstability);

            pipingStructureObserver = CreateFailureMechanismObserver<PipingStructureFailureMechanism,
                PipingStructureFailureMechanismSectionResult>(assessmentSection.PipingStructure);

            strengthStabilityLengthwiseConstructionObserver = CreateFailureMechanismObserver<StrengthStabilityLengthwiseConstructionFailureMechanism,
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(assessmentSection.StrengthStabilityLengthwiseConstruction);

            technicalInnovationObserver = CreateFailureMechanismObserver<TechnicalInnovationFailureMechanism,
                TechnicalInnovationFailureMechanismSectionResult>(assessmentSection.TechnicalInnovation);

            waterPressureAsphaltCoverObserver = CreateFailureMechanismObserver<WaterPressureAsphaltCoverFailureMechanism,
                WaterPressureAsphaltCoverFailureMechanismSectionResult>(assessmentSection.WaterPressureAsphaltCover);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            assessmentSectionObserver.Dispose();
            referenceLineObserver.Dispose();
            closingStructuresObserver.Dispose();
            duneErosionObserver.Dispose();
            grassCoverErosionInwardsObserver.Dispose();
            grassCoverErosionOutwardsObserver.Dispose();
            heightStructuresObserver.Dispose();
            macroStabilityInwardsObserver.Dispose();
            pipingObserver.Dispose();
            stabilityPointStructuresObserver.Dispose();
            stabilityStoneCoverObserver.Dispose();
            waveImpactAsphaltCoverObserver.Dispose();
            grassCoverSlipOffInwardsObserver.Dispose();
            grassCoverSlipOffOutwardsObserver.Dispose();
            macroStabilityOutwardsObserver.Dispose();
            microstabilityObserver.Dispose();
            pipingStructureObserver.Dispose();
            strengthStabilityLengthwiseConstructionObserver.Dispose();
            technicalInnovationObserver.Dispose();
            waterPressureAsphaltCoverObserver.Dispose();
        }

        private void ResubscribeFailureMechanismObservers(AssessmentSection assessmentSection)
        {
            closingStructuresObserver.Observable = assessmentSection.ClosingStructures;
            duneErosionObserver.Observable = assessmentSection.DuneErosion;
            grassCoverErosionInwardsObserver.Observable = assessmentSection.GrassCoverErosionInwards;
            grassCoverErosionOutwardsObserver.Observable = assessmentSection.GrassCoverErosionOutwards;
            heightStructuresObserver.Observable = assessmentSection.HeightStructures;
            macroStabilityInwardsObserver.Observable = assessmentSection.MacroStabilityInwards;
            pipingObserver.Observable = assessmentSection.Piping;
            stabilityPointStructuresObserver.Observable = assessmentSection.StabilityPointStructures;
            stabilityStoneCoverObserver.Observable = assessmentSection.StabilityStoneCover;
            waveImpactAsphaltCoverObserver.Observable = assessmentSection.WaveImpactAsphaltCover;
            grassCoverSlipOffInwardsObserver.Observable = assessmentSection.GrassCoverSlipOffInwards;
            grassCoverSlipOffOutwardsObserver.Observable = assessmentSection.GrassCoverSlipOffOutwards;
            macroStabilityOutwardsObserver.Observable = assessmentSection.MacroStabilityOutwards;
            microstabilityObserver.Observable = assessmentSection.Microstability;
            pipingStructureObserver.Observable = assessmentSection.PipingStructure;
            strengthStabilityLengthwiseConstructionObserver.Observable = assessmentSection.StrengthStabilityLengthwiseConstruction;
            technicalInnovationObserver.Observable = assessmentSection.TechnicalInnovation;
            waterPressureAsphaltCoverObserver.Observable = assessmentSection.WaterPressureAsphaltCover;
        }

        private Observer CreateCalculatableFailureMechanismObserver<TFailureMechanism, TSectionResult, TCalculation>(TFailureMechanism failureMechanism)
            where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>, ICalculatableFailureMechanism
            where TSectionResult : FailureMechanismSectionResult
            where TCalculation : ICalculation<ICalculationInput>
        {
            return new Observer(NotifyObservers)
            {
                Observable = new CalculatableFailureMechanismResultObserver<TFailureMechanism,
                    TSectionResult, TCalculation>(failureMechanism)
            };
        }

        private Observer CreateFailureMechanismObserver<TFailureMechanism, TSectionResult>(TFailureMechanism failureMechanism)
            where TFailureMechanism : IFailureMechanism, IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            return new Observer(NotifyObservers)
            {
                Observable = new FailureMechanismResultObserver<TFailureMechanism,
                    TSectionResult>(failureMechanism)
            };
        }
    }
}