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
using Riskeer.Integration.Data.StandAlone.Input;
using Riskeer.Integration.Data.StandAlone.SectionResults;

namespace Riskeer.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// macro stability outwards failure mechanism.
    /// </summary>
    public class MacroStabilityOutwardsFailureMechanism : FailureMechanismBase,
                                                          IHasSectionResults<MacroStabilityOutwardsFailureMechanismSectionResultOld>
    {
        private readonly ObservableList<MacroStabilityOutwardsFailureMechanismSectionResultOld> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityOutwardsFailureMechanism"/> class.
        /// </summary>
        public MacroStabilityOutwardsFailureMechanism()
            : base(Resources.MacroStabilityOutwardsFailureMechanism_DisplayName, Resources.MacroStabilityOutwardsFailureMechanism_Code, 4)
        {
            sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResultOld>();
            MacroStabilityOutwardsProbabilityAssessmentInput = new MacroStabilityOutwardsProbabilityAssessmentInput();
        }

        /// <summary>
        /// Gets the general probabilistic assessment input parameters that apply to each calculation 
        /// in a semi-probabilistic assessment.
        /// </summary>
        public MacroStabilityOutwardsProbabilityAssessmentInput MacroStabilityOutwardsProbabilityAssessmentInput { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<MacroStabilityOutwardsFailureMechanismSectionResultOld> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            base.AddSectionDependentData(section);
            sectionResults.Add(new MacroStabilityOutwardsFailureMechanismSectionResultOld(section));
        }

        protected override void ClearSectionDependentData()
        {
            sectionResults.Clear();
        }
    }
}