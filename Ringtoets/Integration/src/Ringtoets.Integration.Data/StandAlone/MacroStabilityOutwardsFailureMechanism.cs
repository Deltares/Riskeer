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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone.Input;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// macro stability outwards failure mechanism.
    /// </summary>
    public class MacroStabilityOutwardsFailureMechanism : FailureMechanismBase,
                                                          IHasSectionResults<MacroStabilityOutwardsFailureMechanismSectionResult>
    {
        private readonly ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityOutwardsFailureMechanism"/> class.
        /// </summary>
        public MacroStabilityOutwardsFailureMechanism()
            : base(Resources.MacroStabilityOutwardsFailureMechanism_DisplayName, Resources.MacroStabilityOutwardsFailureMechanism_Code, 4)
        {
            sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>();
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

        public IObservableEnumerable<MacroStabilityOutwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new MacroStabilityOutwardsFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}