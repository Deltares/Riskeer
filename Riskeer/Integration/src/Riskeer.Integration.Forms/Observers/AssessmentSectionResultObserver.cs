﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
        private readonly AssessmentSection assessmentSection;

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
        private readonly Observer microstabilityObserver;
        private readonly Observer pipingStructureObserver;
        private readonly Observer waterPressureAsphaltCoverObserver;

        private readonly Observer specificFailureMechanismsObserver;
        private readonly List<Observer> specificFailureMechanismObservers;

        private readonly RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionConfiguration>,
            PipingFailureMechanismSectionConfiguration> pipingSectionConfigurationsObserver;

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

            this.assessmentSection = assessmentSection;

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
                AdoptableFailureMechanismSectionResult, StructuresCalculation<ClosingStructuresInput>>(assessmentSection.ClosingStructures);

            duneErosionObserver = CreateFailureMechanismObserver<DuneErosionFailureMechanism, NonAdoptableFailureMechanismSectionResult>(assessmentSection.DuneErosion);

            grassCoverErosionInwardsObserver = CreateCalculatableFailureMechanismObserver<GrassCoverErosionInwardsFailureMechanism,
                AdoptableFailureMechanismSectionResult, GrassCoverErosionInwardsCalculation>(assessmentSection.GrassCoverErosionInwards);

            grassCoverErosionOutwardsObserver = CreateFailureMechanismObserver<GrassCoverErosionOutwardsFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.GrassCoverErosionOutwards);

            heightStructuresObserver = CreateCalculatableFailureMechanismObserver<HeightStructuresFailureMechanism,
                AdoptableFailureMechanismSectionResult, StructuresCalculation<HeightStructuresInput>>(assessmentSection.HeightStructures);

            macroStabilityInwardsObserver = CreateCalculatableFailureMechanismObserver<MacroStabilityInwardsFailureMechanism,
                AdoptableFailureMechanismSectionResult, MacroStabilityInwardsCalculationScenario>(assessmentSection.MacroStabilityInwards);

            pipingObserver = CreateCalculatableFailureMechanismObserver<PipingFailureMechanism,
                AdoptableFailureMechanismSectionResult, SemiProbabilisticPipingCalculationScenario>(assessmentSection.Piping);

            stabilityPointStructuresObserver = CreateCalculatableFailureMechanismObserver<StabilityPointStructuresFailureMechanism,
                AdoptableFailureMechanismSectionResult, StructuresCalculation<StabilityPointStructuresInput>>(assessmentSection.StabilityPointStructures);

            stabilityStoneCoverObserver = CreateFailureMechanismObserver<StabilityStoneCoverFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.StabilityStoneCover);

            waveImpactAsphaltCoverObserver = CreateFailureMechanismObserver<WaveImpactAsphaltCoverFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.WaveImpactAsphaltCover);

            grassCoverSlipOffInwardsObserver = CreateFailureMechanismObserver<GrassCoverSlipOffInwardsFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.GrassCoverSlipOffInwards);

            grassCoverSlipOffOutwardsObserver = CreateFailureMechanismObserver<GrassCoverSlipOffOutwardsFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.GrassCoverSlipOffOutwards);

            microstabilityObserver = CreateFailureMechanismObserver<MicrostabilityFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.Microstability);

            pipingStructureObserver = CreateFailureMechanismObserver<PipingStructureFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.PipingStructure);

            waterPressureAsphaltCoverObserver = CreateFailureMechanismObserver<WaterPressureAsphaltCoverFailureMechanism,
                NonAdoptableFailureMechanismSectionResult>(assessmentSection.WaterPressureAsphaltCover);

            specificFailureMechanismsObserver = new Observer(() =>
            {
                ClearSpecificFailureMechanismObservers();
                CreateSpecificFailureMechanismObservers();
                NotifyObservers();
            })
            {
                Observable = assessmentSection.SpecificFailureMechanisms
            };
            specificFailureMechanismObservers = new List<Observer>();
            CreateSpecificFailureMechanismObservers();

            pipingSectionConfigurationsObserver = CreatePipingSectionConfigurationsObserver(assessmentSection.Piping);
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
            microstabilityObserver.Dispose();
            pipingStructureObserver.Dispose();
            waterPressureAsphaltCoverObserver.Dispose();
            specificFailureMechanismsObserver.Dispose();

            ClearSpecificFailureMechanismObservers();

            pipingSectionConfigurationsObserver.Dispose();
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
            microstabilityObserver.Observable = assessmentSection.Microstability;
            pipingStructureObserver.Observable = assessmentSection.PipingStructure;
            waterPressureAsphaltCoverObserver.Observable = assessmentSection.WaterPressureAsphaltCover;
            pipingSectionConfigurationsObserver.Observable = assessmentSection.Piping.SectionConfigurations;
        }

        private Observer CreateCalculatableFailureMechanismObserver<TFailureMechanism, TSectionResult, TCalculation>(TFailureMechanism failureMechanism)
            where TFailureMechanism : IFailureMechanism, IFailureMechanism<TSectionResult>, ICalculatableFailureMechanism
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
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            return new Observer(NotifyObservers)
            {
                Observable = new FailureMechanismResultObserver<TFailureMechanism, TSectionResult>(failureMechanism)
            };
        }

        private void CreateSpecificFailureMechanismObservers()
        {
            IEnumerable<Observer> observers = assessmentSection.SpecificFailureMechanisms.Select(CreateFailureMechanismObserver<SpecificFailureMechanism,
                                                                                                     NonAdoptableFailureMechanismSectionResult>);
            specificFailureMechanismObservers.AddRange(observers);
        }

        private void ClearSpecificFailureMechanismObservers()
        {
            foreach (Observer specificFailureMechanismObserver in specificFailureMechanismObservers)
            {
                specificFailureMechanismObserver.Dispose();
            }

            specificFailureMechanismObservers.Clear();
        }

        private RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionConfiguration>, PipingFailureMechanismSectionConfiguration> CreatePipingSectionConfigurationsObserver(PipingFailureMechanism pipingFailureMechanism)
        {
            return new RecursiveObserver<IObservableEnumerable<PipingFailureMechanismSectionConfiguration>, PipingFailureMechanismSectionConfiguration>(
                NotifyObservers,
                sc => sc)
            {
                Observable = pipingFailureMechanism.SectionConfigurations
            };
        }
    }
}