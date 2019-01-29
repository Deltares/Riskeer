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
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Piping.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingCalculationScenario"/> in the <see cref="PipingScenariosView"/>.
    /// </summary>
    internal class PipingScenarioRow
    {
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private DerivedPipingOutput derivedOutput;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculationScenario"/> this row contains.</param>
        /// <param name="failureMechanism">The failure mechanism that the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingScenarioRow(PipingCalculationScenario calculation, PipingFailureMechanism failureMechanism,
                                 IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Calculation = calculation;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            CreateDerivedOutput();
        }

        /// <summary>
        /// Gets the <see cref="PipingCalculationScenario"/> this row contains.
        /// </summary>
        public PipingCalculationScenario Calculation { get; }

        /// <summary>
        /// Gets or sets the <see cref="PipingCalculationScenario"/> is relevant.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return Calculation.IsRelevant;
            }
            set
            {
                Calculation.IsRelevant = value;
                Calculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the contribution of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public RoundedDouble Contribution
        {
            get
            {
                return new RoundedDouble(2, Calculation.Contribution * 100);
            }
            set
            {
                Calculation.Contribution = (RoundedDouble) (value / 100);
                Calculation.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the name of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return Calculation.Name;
            }
        }

        /// <summary>
        /// Gets the failure probability of piping of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public string FailureProbabilityPiping
        {
            get
            {
                return derivedOutput == null
                           ? RingtoetsCommonFormsResources.RoundedDouble_No_result_dash
                           : ProbabilityFormattingHelper.Format(derivedOutput.PipingProbability);
            }
        }

        /// <summary>
        /// Gets the failure probability of uplift sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public string FailureProbabilityUplift
        {
            get
            {
                return derivedOutput == null
                           ? RingtoetsCommonFormsResources.RoundedDouble_No_result_dash
                           : ProbabilityFormattingHelper.Format(derivedOutput.UpliftProbability);
            }
        }

        /// <summary>
        /// Gets the failure probability of heave sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public string FailureProbabilityHeave
        {
            get
            {
                return derivedOutput == null
                           ? RingtoetsCommonFormsResources.RoundedDouble_No_result_dash
                           : ProbabilityFormattingHelper.Format(derivedOutput.HeaveProbability);
            }
        }

        /// <summary>
        /// Gets the failure probability of sellmeijer sub failure mechanism of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public string FailureProbabilitySellmeijer
        {
            get
            {
                return derivedOutput == null
                           ? RingtoetsCommonFormsResources.RoundedDouble_No_result_dash
                           : ProbabilityFormattingHelper.Format(derivedOutput.SellmeijerProbability);
            }
        }

        /// <summary>
        /// Updates the row based on the current output of the calculation scenario.
        /// </summary>
        public void Update()
        {
            CreateDerivedOutput();
        }

        private void CreateDerivedOutput()
        {
            derivedOutput = Calculation.HasOutput
                                ? DerivedPipingOutputFactory.Create(Calculation.Output, failureMechanism, assessmentSection)
                                : null;
        }
    }
}