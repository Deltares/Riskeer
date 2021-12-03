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
using System.ComponentModel;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public class PipingFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<PipingFailureMechanismSectionResult>
    {
        private readonly IPipingFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="PipingFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculateProbabilityStrategy">The strategy used to calculate probabilities.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        internal PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult,
                                                        IPipingFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy)
            : base(sectionResult)
        {
            if (calculateProbabilityStrategy == null)
            {
                throw new ArgumentNullException(nameof(calculateProbabilityStrategy));
            }

            this.calculateProbabilityStrategy = calculateProbabilityStrategy;
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
                       ? calculateProbabilityStrategy.CalculateProfileProbability()
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
                       ? calculateProbabilityStrategy.CalculateSectionProbability()
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
        public double RefinedProfileProbability
        {
            get => SectionResult.RefinedProfileProbability;
            set
            {
                SectionResult.RefinedProfileProbability = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the refined probability per failure mechanism section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>\
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double RefinedSectionProbability
        {
            get => SectionResult.RefinedSectionProbability;
            set
            {
                SectionResult.RefinedSectionProbability = value;
                UpdateInternalData();
            }
        }

        public override void Update() {}
    }
}