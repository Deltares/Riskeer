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
using System.Collections.Generic;
using System.ComponentModel;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using CommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<MacroStabilityInwardsFailureMechanismSectionResult>
    {
        private readonly IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios;
        private readonly MacroStabilityInwardsFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculationScenarios">All calculation scenarios in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismSectionResultRow(MacroStabilityInwardsFailureMechanismSectionResult sectionResult,
                                                                     IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios,
                                                                     MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(sectionResult)
        {
            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.calculationScenarios = calculationScenarios;
            this.failureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets or sets whether the section is relevant.
        /// </summary>
        public bool IsRelevant
        {
            get => SectionResult.IsRelevant;
            set
            {
                SectionResult.IsRelevant = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the initial failure mechanism result.
        /// </summary>
        public InitialFailureMechanismResultType InitialFailureMechanismResult
        {
            get => SectionResult.InitialFailureMechanismResult;
            set
            {
                SectionResult.InitialFailureMechanismResult = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the initial failure mechanism result per profile as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double InitialFailureMechanismResultProfileProbability
        {
            get => SectionResult.InitialFailureMechanismResult == InitialFailureMechanismResultType.Adopt
                       ? SectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios, failureMechanism.GeneralInput.ModelFactor)
                       : SectionResult.ManualInitialFailureMechanismResultProfileProbability;
            set
            {
                SectionResult.ManualInitialFailureMechanismResultProfileProbability = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the initial failure mechanism result per failure mechanism section as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double InitialFailureMechanismResultSectionProbability
        {
            get => SectionResult.InitialFailureMechanismResult == InitialFailureMechanismResultType.Adopt
                       ? SectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios, failureMechanism.GeneralInput.ModelFactor)
                         * failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(SectionResult.Section.Length)
                       : SectionResult.ManualInitialFailureMechanismResultSectionProbability;
            set
            {
                SectionResult.ManualInitialFailureMechanismResultSectionProbability = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets whether further analysis is needed.
        /// </summary>
        public bool FurtherAnalysisNeeded
        {
            get => SectionResult.FurtherAnalysisNeeded;
            set
            {
                SectionResult.FurtherAnalysisNeeded = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the probability refinement type.
        /// </summary>
        public ProbabilityRefinementType ProbabilityRefinementType
        {
            get => SectionResult.ProbabilityRefinementType;
            set
            {
                SectionResult.ProbabilityRefinementType = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the refined probability per profile.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public object RefinedProfileProbability
        {
            get => ProbabilityRefinementType == ProbabilityRefinementType.Section
                       ? (object) CommonFormsResources.FailureMechanismSectionResultRow_Derived_DisplayName
                       : SectionResult.RefinedProfileProbability;
            set
            {
                SectionResult.RefinedProfileProbability = (double) value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the refined probability per failure mechanism section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>\
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public object RefinedSectionProbability
        {
            get => ProbabilityRefinementType == ProbabilityRefinementType.Profile
                       ? (object) CommonFormsResources.FailureMechanismSectionResultRow_Derived_DisplayName
                       : SectionResult.RefinedSectionProbability;
            set
            {
                SectionResult.RefinedSectionProbability = (double) value;
                UpdateInternalData();
            }
        }

        public override void Update()
        {
        }
    }
}