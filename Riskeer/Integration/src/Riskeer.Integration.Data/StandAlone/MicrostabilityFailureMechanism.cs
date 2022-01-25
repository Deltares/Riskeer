﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Microstability failure mechanism.
    /// </summary>
    public class MicrostabilityFailureMechanism : FailureMechanismBase, IHasSectionResults<MicrostabilityFailureMechanismSectionResultOld, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>,
                                                  IHasGeneralInput
    {
        private readonly ObservableList<MicrostabilityFailureMechanismSectionResultOld> sectionResultsOld;
        private readonly ObservableList<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrostabilityFailureMechanism"/> class.
        /// </summary>
        public MicrostabilityFailureMechanism()
            : base(Resources.MicrostabilityFailureMechanism_DisplayName, Resources.MicrostabilityFailureMechanism_Code, 4)
        {
            sectionResultsOld = new ObservableList<MicrostabilityFailureMechanismSectionResultOld>();
            sectionResults = new ObservableList<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();
            GeneralInput = new GeneralInput();
        }

        public GeneralInput GeneralInput { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<MicrostabilityFailureMechanismSectionResultOld> SectionResultsOld => sectionResultsOld;

        public IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> SectionResults => sectionResults;

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            base.AddSectionDependentData(section);
            sectionResultsOld.Add(new MicrostabilityFailureMechanismSectionResultOld(section));
            sectionResults.Add(new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionDependentData()
        {
            sectionResultsOld.Clear();
            sectionResults.Clear();
        }
    }
}