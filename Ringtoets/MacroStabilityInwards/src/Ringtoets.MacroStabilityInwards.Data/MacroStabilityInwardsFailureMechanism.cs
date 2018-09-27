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
using Ringtoets.MacroStabilityInwards.Data.Properties;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Model for performing macro stability inwards calculations.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanism : FailureMechanismBase,
                                                         ICalculatableFailureMechanism,
                                                         IHasSectionResults<MacroStabilityInwardsFailureMechanismSectionResult>
    {
        private readonly ObservableList<MacroStabilityInwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsFailureMechanism"/> class.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism()
            : base(Resources.MacroStabilityInwardsFailureMechanism_DisplayName, Resources.MacroStabilityInwardsFailureMechanism_Code, 2)
        {
            GeneralInput = new GeneralMacroStabilityInwardsInput();
            MacroStabilityInwardsProbabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput();
            SurfaceLines = new MacroStabilityInwardsSurfaceLineCollection();
            StochasticSoilModels = new MacroStabilityInwardsStochasticSoilModelCollection();
            CalculationsGroup = new CalculationGroup
            {
                Name = RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName
            };

            sectionResults = new ObservableList<MacroStabilityInwardsFailureMechanismSectionResult>();
        }

        /// <summary>
        /// Gets the available surface lines within the scope of the macro stability inwards failure mechanism.
        /// </summary>
        public MacroStabilityInwardsSurfaceLineCollection SurfaceLines { get; }

        /// <summary>
        /// Gets the available stochastic soil models within the scope of the macro stability inwards failure mechanism.
        /// </summary>
        public MacroStabilityInwardsStochasticSoilModelCollection StochasticSoilModels { get; }

        /// <summary>
        /// Gets the general calculation input parameters that apply to each macro stability inwards calculation.
        /// </summary>
        public GeneralMacroStabilityInwardsInput GeneralInput { get; }

        /// <summary>
        /// Gets the general probabilistic assessment input parameters that apply to each calculation 
        /// in a semi-probabilistic assessment.
        /// </summary>
        public MacroStabilityInwardsProbabilityAssessmentInput MacroStabilityInwardsProbabilityAssessmentInput { get; }

        public CalculationGroup CalculationsGroup { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations();
            }
        }

        public IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new MacroStabilityInwardsFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}