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
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Forms.Views.SectionResultRows
{
    /// <summary>
    /// Class for displaying <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>  as a row in a grid view.
    /// </summary>
    public class MacroStabilityOutwardsSectionResultRow : FailureMechanismSectionResultRow<MacroStabilityOutwardsFailureMechanismSectionResult>
    {
        private readonly MacroStabilityOutwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityOutwardsSectionResultRow(MacroStabilityOutwardsFailureMechanismSectionResult sectionResult,
                                                      MacroStabilityOutwardsFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection) : base(sectionResult)
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
        public SimpleAssessmentResultType SimpleAssessmentResult
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
        /// Gets or sets the detailed assessment probability of the <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is 
        /// not in the range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DetailedAssessmentProbability
        {
            get
            {
                return SectionResult.DetailedAssessmentProbability;
            }
            set
            {
                SectionResult.DetailedAssessmentProbability = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the value representing the tailor made assessment result.
        /// </summary>
        public TailorMadeAssessmentResultType TailorMadeAssessmentResult
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
        /// Gets or sets the tailor made assessment probability of the <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
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
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup SimpleAssemblyCategoryGroup
        {
            get
            {
                return MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(SectionResult).Group;
            }
        }

        /// <summary>
        /// Gets the detailed assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup DetailedAssemblyCategoryGroup
        {
            get
            {
                return MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
        }

        /// <summary>
        /// Gets the tailor made assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup TailorMadeAssemblyCategoryGroup
        {
            get
            {
                return MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
        }

        /// <summary>
        /// Gets the combined assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedAssemblyCategoryGroup
        {
            get
            {
                return MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
        }

        /// <summary>
        /// Gets or sets the indicator whether the combined assembly should be overwritten by <see cref="ManualAssemblyCategoryGroup"/>.
        /// </summary>
        public bool UseManualAssemblyCategoryGroup
        {
            get
            {
                return SectionResult.UseManualAssemblyCategoryGroup;
            }
            set
            {
                SectionResult.UseManualAssemblyCategoryGroup = value;
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the manually selected assembly category group.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup ManualAssemblyCategoryGroup
        {
            get
            {
                return SectionResult.ManualAssemblyCategoryGroup;
            }
            set
            {
                SectionResult.ManualAssemblyCategoryGroup = value;
                SectionResult.NotifyObservers();
            }
        }
    }
}