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

using System.Collections.Generic;
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Properties;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Piping Structure failure mechanism.
    /// </summary>
    public class PipingStructureFailureMechanism : FailureMechanismBase,
                                                   IHasSectionResults<PipingStructureFailureMechanismSectionResultOld, NonAdoptableFailureMechanismSectionResult>,
                                                   IHasGeneralInput
    {
        private readonly ObservableList<PipingStructureFailureMechanismSectionResultOld> sectionResultsOld;
        private readonly ObservableList<NonAdoptableFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingStructureFailureMechanism"/> class.
        /// </summary>
        public PipingStructureFailureMechanism()
            : base(Resources.PipingStructureFailureMechanism_DisplayName, Resources.PipingStructureFailureMechanism_Code, 4)
        {
            sectionResults = new ObservableList<NonAdoptableFailureMechanismSectionResult>();
            sectionResultsOld = new ObservableList<PipingStructureFailureMechanismSectionResultOld>();
            GeneralInput = new GeneralInput
            {
                ApplyLengthEffectInSection = false
            };
        }

        public GeneralInput GeneralInput { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<PipingStructureFailureMechanismSectionResultOld> SectionResultsOld => sectionResultsOld;

        public IObservableEnumerable<NonAdoptableFailureMechanismSectionResult> SectionResults => sectionResults;

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            base.AddSectionDependentData(section);
            sectionResultsOld.Add(new PipingStructureFailureMechanismSectionResultOld(section));
            sectionResults.Add(new NonAdoptableFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionDependentData()
        {
            sectionResultsOld.Clear();
            sectionResults.Clear();
        }
    }
}