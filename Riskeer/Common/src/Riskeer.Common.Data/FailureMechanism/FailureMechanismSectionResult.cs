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
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Properties;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Base class for classes that hold information of the result of the <see cref="FailureMechanismSection"/>.
    /// </summary>
    public abstract class FailureMechanismSectionResult : Observable
    {
        private double manualInitialFailureMechanismResultSectionProbability;
        private double refinedSectionProbability;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        protected FailureMechanismSectionResult(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Section = section;
            IsRelevant = true;
            ManualInitialFailureMechanismResultSectionProbability = double.NaN;
            FurtherAnalysisType = FailureMechanismSectionResultFurtherAnalysisType.NotNecessary;
            RefinedSectionProbability = double.NaN;
        }

        /// <summary>
        /// Gets or sets whether the section is relevant.
        /// </summary>
        public bool IsRelevant { get; set; }

        /// <summary>
        /// Gets or sets the value of the manual initial failure mechanism result per failure mechanism section as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in a valid range.</exception>
        public double ManualInitialFailureMechanismResultSectionProbability
        {
            get => manualInitialFailureMechanismResultSectionProbability;
            set
            {
                ValidateInitialSectionProbability(value);
                manualInitialFailureMechanismResultSectionProbability = value;
            }
        }

        /// <summary>
        /// Gets or sets whether further analysis is needed.
        /// </summary>
        public bool FurtherAnalysisNeeded { get; set; }

        /// <summary>
        /// Gets or sets the further analysis type.
        /// </summary>
        public FailureMechanismSectionResultFurtherAnalysisType FurtherAnalysisType { get; set; }

        /// <summary>
        /// Gets or sets the value of the refined probability per failure mechanism section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in a valid range.</exception>
        public double RefinedSectionProbability
        {
            get => refinedSectionProbability;
            set
            {
                ValidateRefinedSectionProbability(value);
                refinedSectionProbability = value;
            }
        }

        /// <summary>
        /// Gets the encapsulated <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; }

        /// <summary>
        /// Validates the initial section probability.
        /// </summary>
        /// <param name="value">The probability to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in a valid range.</exception>
        protected virtual void ValidateInitialSectionProbability(double value)
        {
            ValidateFailureProbability(value);
        }

        /// <summary>
        /// Validates the refined section probability.
        /// </summary>
        /// <param name="value">The probability to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in a valid range.</exception>
        protected virtual void ValidateRefinedSectionProbability(double value)
        {
            ValidateFailureProbability(value);
        }

        /// <summary>
        /// Validates the failure probability.
        /// </summary>
        /// <param name="value">The probability to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        protected static void ValidateFailureProbability(double value)
        {
            ProbabilityHelper.ValidateProbability(value, null,
                                                  Resources.FailureProbability_Value_needs_to_be_in_Range_0_,
                                                  true);
        }
    }
}