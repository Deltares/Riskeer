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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data.SoilProfile;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using PipingDataResources = Riskeer.Piping.Data.Properties.Resources;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism : FailureMechanismBase, ICalculatableFailureMechanism, IHasSectionResults<PipingFailureMechanismSectionResult>
    {
        private readonly ObservableList<PipingFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanism"/> class.
        /// </summary>
        public PipingFailureMechanism()
            : base(PipingDataResources.PipingFailureMechanism_DisplayName, PipingDataResources.PipingFailureMechanism_DisplayCode, 2)
        {
            PipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();
            GeneralInput = new GeneralPipingInput();
            SurfaceLines = new PipingSurfaceLineCollection();
            StochasticSoilModels = new PipingStochasticSoilModelCollection();
            CalculationsGroup = new CalculationGroup
            {
                Name = RiskeerCommonDataResources.FailureMechanism_Calculations_DisplayName
            };

            sectionResults = new ObservableList<PipingFailureMechanismSectionResult>();
        }

        /// <summary>
        /// Gets the available surface lines within the scope of the piping failure mechanism.
        /// </summary>
        public PipingSurfaceLineCollection SurfaceLines { get; }

        /// <summary>
        /// Gets the available stochastic soil models within the scope of the piping failure mechanism.
        /// </summary>
        public PipingStochasticSoilModelCollection StochasticSoilModels { get; }

        /// <summary>
        /// Gets the general calculation input parameters that apply to each piping calculation.
        /// </summary>
        public GeneralPipingInput GeneralInput { get; }

        /// <summary>
        /// Gets the general probabilistic assessment input parameters that apply to each calculation 
        /// in a semi-probabilistic assessment.
        /// </summary>
        public PipingProbabilityAssessmentInput PipingProbabilityAssessmentInput { get; }

        public CalculationGroup CalculationsGroup { get; }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations();
            }
        }

        public IObservableEnumerable<PipingFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new PipingFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}