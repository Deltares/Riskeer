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
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Helpers;
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
        private readonly int initialFailureMechanismResultIndex;
        private readonly int initialFailureMechanismResultProfileProbabilityIndex;
        private readonly int initialFailureMechanismResultSectionProbabilityIndex;
        private readonly int furtherAnalysisNeededIndex;
        private readonly int probabilityRefinementTypeIndex;
        private readonly int refinedProfileProbabilityIndex;
        private readonly int refinedSectionProbabilityIndex;
        private readonly int profileProbabilityIndex;
        private readonly int sectionProbabilityIndex;
        private readonly int sectionNIndex;
        private readonly int assemblyGroupIndex;

        private readonly IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios;
        private readonly MacroStabilityInwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculationScenarios">All calculation scenarios in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="MacroStabilityInwardsFailureMechanismSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismSectionResultRow(MacroStabilityInwardsFailureMechanismSectionResult sectionResult,
                                                                     IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios,
                                                                     MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                     IAssessmentSection assessmentSection,
                                                                     ConstructionProperties constructionProperties)
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

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            this.calculationScenarios = calculationScenarios;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            initialFailureMechanismResultIndex = constructionProperties.InitialFailureMechanismResultIndex;
            initialFailureMechanismResultProfileProbabilityIndex = constructionProperties.InitialFailureMechanismResultProfileProbabilityIndex;
            initialFailureMechanismResultSectionProbabilityIndex = constructionProperties.InitialFailureMechanismResultSectionProbabilityIndex;
            furtherAnalysisNeededIndex = constructionProperties.FurtherAnalysisNeededIndex;
            probabilityRefinementTypeIndex = constructionProperties.ProbabilityRefinementTypeIndex;
            refinedProfileProbabilityIndex = constructionProperties.RefinedProfileProbabilityIndex;
            refinedSectionProbabilityIndex = constructionProperties.RefinedSectionProbabilityIndex;
            profileProbabilityIndex = constructionProperties.ProfileProbabilityIndex;
            sectionProbabilityIndex = constructionProperties.SectionProbabilityIndex;
            sectionNIndex = constructionProperties.SectionNIndex;
            assemblyGroupIndex = constructionProperties.AssemblyGroupIndex;

            Update();
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

        /// <summary>
        /// Gets the profile probability.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double ProfileProbability => AssemblyResult.ProfileProbability;

        /// <summary>
        /// Gets the section probability.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double SectionProbability => AssemblyResult.SectionProbability;

        /// <summary>
        /// Gets the section N.
        /// </summary>
        [TypeConverter(typeof(NoValueDoubleConverter))]
        public double SectionN => AssemblyResult.N;

        /// <summary>
        /// Gets the assembly group.
        /// </summary>
        public string AssemblyGroup => FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(AssemblyResult.AssemblyGroup);

        public override void Update()
        {
            UpdateDerivedData();
        }
        
        private void UpdateDerivedData()
        {
            TryGetAssemblyResult();
        }
        
        private void TryGetAssemblyResult()
        {
            try
            {
                AssemblyResult = FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                    assessmentSection, IsRelevant, InitialFailureMechanismResult, InitialFailureMechanismResultProfileProbability,
                    InitialFailureMechanismResultSectionProbability, FurtherAnalysisNeeded,
                    SectionResult.RefinedProfileProbability, SectionResult.RefinedSectionProbability,
                    ProbabilityRefinementType, () => failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(SectionResult.Section.Length));
            }
            catch (AssemblyException e)
            {
                AssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
                ColumnStateDefinitions[profileProbabilityIndex].ErrorText = e.Message;
                ColumnStateDefinitions[sectionProbabilityIndex].ErrorText = e.Message;
                ColumnStateDefinitions[sectionNIndex].ErrorText = e.Message;
                ColumnStateDefinitions[assemblyGroupIndex].ErrorText = e.Message;
            }
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="MacroStabilityInwardsFailureMechanismSectionResultRow"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Sets the initial failure mechanism result index.
            /// </summary>
            public int InitialFailureMechanismResultIndex { internal get; set; }

            /// <summary>
            /// Sets the initial failure mechanism result profile probability index.
            /// </summary>
            public int InitialFailureMechanismResultProfileProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the initial failure mechanism result section probability index.
            /// </summary>
            public int InitialFailureMechanismResultSectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the further analysis needed index.
            /// </summary>
            public int FurtherAnalysisNeededIndex { internal get; set; }

            /// <summary>
            /// Sets the probability refinement type index.
            /// </summary>
            public int ProbabilityRefinementTypeIndex { internal get; set; }

            /// <summary>
            /// Sets the refined profile probability index.
            /// </summary>
            public int RefinedProfileProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the refined section probability index.
            /// </summary>
            public int RefinedSectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the profile probability index.
            /// </summary>
            public int ProfileProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the section probability index.
            /// </summary>
            public int SectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the section N index.
            /// </summary>
            public int SectionNIndex { internal get; set; }

            /// <summary>
            /// Sets the assembly group index.
            /// </summary>
            public int AssemblyGroupIndex { internal get; set; }
        }
    }
}