// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>, which takes care of the
    /// representation of properties in a grid.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<GrassCoverErosionInwardsFailureMechanismSectionResult>
    {
        private readonly GrassCoverErosionInwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="failureMechanism">The failure mechanism the result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the result belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal GrassCoverErosionInwardsFailureMechanismSectionResultRow(GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult,
                                                                          GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                          IAssessmentSection assessmentSection)
            : base(sectionResult)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets or sets the value representing the simple assessment result.
        /// </summary>
        public SimpleAssessmentResultValidityOnlyType SimpleAssessmentResult
        {
            get
            {
                return SectionResult.SimpleAssessmentResult;
            }
            set
            {
                SectionResult.SimpleAssessmentResult = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the value representing the detailed assessment result.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResult
        {
            get
            {
                return SectionResult.DetailedAssessmentResult;
            }
            set
            {
                SectionResult.DetailedAssessmentResult = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the value representing the result of the detailed assessment.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DetailedAssessmentProbability
        {
            get
            {
                return SectionResult.GetDetailedAssessmentProbability(failureMechanism, assessmentSection);
            }
        }

        /// <summary>
        /// Gets or sets the value representing the tailor made assessment result.
        /// </summary>
        public TailorMadeAssessmentProbabilityCalculationResultType TailorMadeAssessmentResult
        {
            get
            {
                return SectionResult.TailorMadeAssessmentResult;
            }
            set
            {
                SectionResult.TailorMadeAssessmentResult = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the tailor made assessment probability of the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is 
        /// not in the range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TailorMadeAssessmentProbability
        {
            get
            {
                return SectionResult.TailorMadeAssessmentProbability;
            }
            set
            {
                SectionResult.TailorMadeAssessmentProbability = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the simple assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be calculated.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup SimpleAssemblyCategoryGroup
        {
            get
            {
                return GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(SectionResult).Group;
            }
        }

        /// <summary>
        /// Gets the detailed assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be calculated.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup DetailedAssemblyCategoryGroup
        {
            get
            {
                return GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
        }

        /// <summary>
        /// Gets the tailor made assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be calculated.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup TailorMadeAssemblyCategoryGroup
        {
            get
            {
                return GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
        }

        /// <summary>
        /// Gets the combined assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be calculated.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedAssemblyCategoryGroup
        {
            get
            {
                return GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
        }

        /// <summary>
        /// Gets the combined assembly probability.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the probability could not be calculated.</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CombinedAssemblyProbability
        {
            get
            {
                return GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Probability;
            }
        }

        /// <summary>
        /// Gets or sets the indicator whether the combined assembly probability
        /// should be overwritten by <see cref="ManualAssemblyProbability"/>.
        /// </summary>
        public bool UseManualAssemblyProbability
        {
            get
            {
                return SectionResult.UseManualAssemblyProbability;
            }
            set
            {
                SectionResult.UseManualAssemblyProbability = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the manually entered assembly probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is 
        /// not in the range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double ManualAssemblyProbability
        {
            get
            {
                return SectionResult.ManualAssemblyProbability;
            }
            set
            {
                SectionResult.ManualAssemblyProbability = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the <see cref="GrassCoverErosionInwardsCalculation"/> of the wrapped
        /// <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <returns><c>null</c> if the wrapped section result does not have a calculation
        /// set. Otherwise the calculation of the wrapped section result is returned.</returns>
        public GrassCoverErosionInwardsCalculation GetSectionResultCalculation()
        {
            return SectionResult.Calculation;
        }
    }
}