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
using Core.Common.Base;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Observers;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Forms.Observers
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
                ClosingStructuresFailureMechanismSectionResultOld, StructuresCalculation<ClosingStructuresInput>>(assessmentSection.ClosingStructures);

            duneErosionObserver = CreateFailureMechanismObserver<DuneErosionFailureMechanism,
                DuneErosionFailureMechanismSectionResultOld>(assessmentSection.DuneErosion);

            grassCoverErosionInwardsObserver = CreateCalculatableFailureMechanismObserver<GrassCoverErosionInwardsFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismSectionResultOld, GrassCoverErosionInwardsCalculation>(assessmentSection.GrassCoverErosionInwards);

            grassCoverErosionOutwardsObserver = CreateFailureMechanismObserver<GrassCoverErosionOutwardsFailureMechanism,
                GrassCoverErosionOutwardsFailureMechanismSectionResultOld>(assessmentSection.GrassCoverErosionOutwards);

            heightStructuresObserver = CreateCalculatableFailureMechanismObserver<HeightStructuresFailureMechanism,
                HeightStructuresFailureMechanismSectionResultOld, StructuresCalculation<HeightStructuresInput>>(assessmentSection.HeightStructures);

            macroStabilityInwardsObserver = CreateCalculatableFailureMechanismObserver<MacroStabilityInwardsFailureMechanism,
                MacroStabilityInwardsFailureMechanismSectionResultOld, MacroStabilityInwardsCalculationScenario>(assessmentSection.MacroStabilityInwards);

            pipingObserver = CreateCalculatableFailureMechanismObserver<PipingFailureMechanism,
                PipingFailureMechanismSectionResultOld, SemiProbabilisticPipingCalculationScenario>(assessmentSection.Piping);

            stabilityPointStructuresObserver = CreateCalculatableFailureMechanismObserver<StabilityPointStructuresFailureMechanism,
                StabilityPointStructuresFailureMechanismSectionResultOld, StructuresCalculation<StabilityPointStructuresInput>>(assessmentSection.StabilityPointStructures);

            stabilityStoneCoverObserver = CreateFailureMechanismObserver<StabilityStoneCoverFailureMechanism,
                StabilityStoneCoverFailureMechanismSectionResultOld>(assessmentSection.StabilityStoneCover);

            waveImpactAsphaltCoverObserver = CreateFailureMechanismObserver<WaveImpactAsphaltCoverFailureMechanism,
                WaveImpactAsphaltCoverFailureMechanismSectionResultOld>(assessmentSection.WaveImpactAsphaltCover);

            grassCoverSlipOffInwardsObserver = CreateFailureMechanismObserver<GrassCoverSlipOffInwardsFailureMechanism,
                GrassCoverSlipOffInwardsFailureMechanismSectionResultOld>(assessmentSection.GrassCoverSlipOffInwards);

            grassCoverSlipOffOutwardsObserver = CreateFailureMechanismObserver<GrassCoverSlipOffOutwardsFailureMechanism,
                GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld>(assessmentSection.GrassCoverSlipOffOutwards);

            macroStabilityOutwardsObserver = CreateFailureMechanismObserver<MacroStabilityOutwardsFailureMechanism,
                MacroStabilityOutwardsFailureMechanismSectionResultOld>(assessmentSection.MacroStabilityOutwards);

            microstabilityObserver = CreateFailureMechanismObserver<MicrostabilityFailureMechanism,
                MicrostabilityFailureMechanismSectionResultOld>(assessmentSection.Microstability);

            pipingStructureObserver = CreateFailureMechanismObserver<PipingStructureFailureMechanism,
                PipingStructureFailureMechanismSectionResultOld>(assessmentSection.PipingStructure);

            strengthStabilityLengthwiseConstructionObserver = CreateFailureMechanismObserver<StrengthStabilityLengthwiseConstructionFailureMechanism,
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultOld>(assessmentSection.StrengthStabilityLengthwiseConstruction);

            technicalInnovationObserver = CreateFailureMechanismObserver<TechnicalInnovationFailureMechanism,
                TechnicalInnovationFailureMechanismSectionResultOld>(assessmentSection.TechnicalInnovation);

            waterPressureAsphaltCoverObserver = CreateFailureMechanismObserver<WaterPressureAsphaltCoverFailureMechanism,
                WaterPressureAsphaltCoverFailureMechanismSectionResultOld>(assessmentSection.WaterPressureAsphaltCover);
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
            where TSectionResult : FailureMechanismSectionResultOld
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
            where TSectionResult : FailureMechanismSectionResultOld
        {
            return new Observer(NotifyObservers)
            {
                Observable = new FailureMechanismResultObserver<TFailureMechanism,
                    TSectionResult>(failureMechanism)
            };
        }
    }
}