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
using Core.Common.Controls.DataGrid;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.TypeConverters;
using CommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.
    /// </summary>
    public class AdoptableWithProfileProbabilityFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<AdoptableWithProfileProbabilityFailureMechanismSectionResult>
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

        private readonly IFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy;
        private readonly IInitialFailureMechanismResultErrorProvider initialFailureMechanismResultErrorProvider;
        private readonly ILengthEffectProvider lengthEffectProvider;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculateProbabilityStrategy">The strategy used to calculate probabilities.</param>
        /// <param name="initialFailureMechanismResultErrorProvider">The error provider to use for
        /// the initial failure mechanism result.</param>
        /// <param name="lengthEffectProvider">The provider to get the length effect properties.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public AdoptableWithProfileProbabilityFailureMechanismSectionResultRow(AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                               IFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy,
                                                                               IInitialFailureMechanismResultErrorProvider initialFailureMechanismResultErrorProvider,
                                                                               ILengthEffectProvider lengthEffectProvider,
                                                                               IAssessmentSection assessmentSection,
                                                                               ConstructionProperties constructionProperties)
            : base(sectionResult)
        {
            if (calculateProbabilityStrategy == null)
            {
                throw new ArgumentNullException(nameof(calculateProbabilityStrategy));
            }

            if (initialFailureMechanismResultErrorProvider == null)
            {
                throw new ArgumentNullException(nameof(initialFailureMechanismResultErrorProvider));
            }

            if (lengthEffectProvider == null)
            {
                throw new ArgumentNullException(nameof(lengthEffectProvider));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            this.calculateProbabilityStrategy = calculateProbabilityStrategy;
            this.initialFailureMechanismResultErrorProvider = initialFailureMechanismResultErrorProvider;
            this.lengthEffectProvider = lengthEffectProvider;
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

            CreateColumnStateDefinitions();

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
        public AdoptableInitialFailureMechanismResultType InitialFailureMechanismResult
        {
            get => SectionResult.InitialFailureMechanismResultType;
            set
            {
                SectionResult.InitialFailureMechanismResultType = value;
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
            get => SectionResult.InitialFailureMechanismResultType == AdoptableInitialFailureMechanismResultType.Adopt
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
            get => SectionResult.InitialFailureMechanismResultType == AdoptableInitialFailureMechanismResultType.Adopt
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
            UpdateColumnStateDefinitions();
            UpdateInitialFailureMechanismResultErrors();
        }

        private void UpdateInitialFailureMechanismResultErrors()
        {
            if (SectionResult.IsRelevant && SectionResult.InitialFailureMechanismResultType == AdoptableInitialFailureMechanismResultType.Adopt)
            {
                ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex].ErrorText = initialFailureMechanismResultErrorProvider.GetProbabilityValidationError(
                    calculateProbabilityStrategy.CalculateProfileProbability);
                ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex].ErrorText = initialFailureMechanismResultErrorProvider.GetProbabilityValidationError(
                    calculateProbabilityStrategy.CalculateSectionProbability);
            }
        }

        private void UpdateDerivedData()
        {
            ResetErrorTexts();
            TryGetAssemblyResult();
        }

        private void ResetErrorTexts()
        {
            ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[profileProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[sectionProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[sectionNIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[assemblyGroupIndex].ErrorText = string.Empty;
        }

        private void TryGetAssemblyResult()
        {
            try
            {
                if (lengthEffectProvider.UseLengthEffect)
                {
                    AssemblyResult = FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                        assessmentSection, IsRelevant, InitialFailureMechanismResult, InitialFailureMechanismResultProfileProbability,
                        InitialFailureMechanismResultSectionProbability, FurtherAnalysisNeeded,
                        SectionResult.RefinedProfileProbability, SectionResult.RefinedSectionProbability,
                        ProbabilityRefinementType, lengthEffectProvider.SectionN);
                }
                else
                {
                    AssemblyResult = FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                        assessmentSection, IsRelevant, InitialFailureMechanismResult,
                        InitialFailureMechanismResultSectionProbability, FurtherAnalysisNeeded,
                        SectionResult.RefinedSectionProbability);
                }
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

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(initialFailureMechanismResultIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(initialFailureMechanismResultProfileProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(initialFailureMechanismResultSectionProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(furtherAnalysisNeededIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(probabilityRefinementTypeIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(refinedProfileProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(refinedSectionProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(profileProbabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(sectionProbabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(sectionNIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(assemblyGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void UpdateColumnStateDefinitions()
        {
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[initialFailureMechanismResultIndex], !IsRelevant);

            if (!IsRelevant || InitialFailureMechanismResult == AdoptableInitialFailureMechanismResultType.NoFailureProbability)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex]);
            }
            else
            {
                bool initialFailureMechanismResultAdopt = InitialFailureMechanismResult == AdoptableInitialFailureMechanismResultType.Adopt;
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex], initialFailureMechanismResultAdopt);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex], initialFailureMechanismResultAdopt);
            }

            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[furtherAnalysisNeededIndex], !IsRelevant);
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[probabilityRefinementTypeIndex], !IsRelevant || !FurtherAnalysisNeeded);

            if (!IsRelevant || !FurtherAnalysisNeeded)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[refinedProfileProbabilityIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[refinedSectionProbabilityIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[refinedProfileProbabilityIndex], ProbabilityRefinementType == ProbabilityRefinementType.Section);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[refinedSectionProbabilityIndex], ProbabilityRefinementType == ProbabilityRefinementType.Profile);
            }

            FailureMechanismSectionResultRowHelper.SetAssemblyGroupStyle(ColumnStateDefinitions[assemblyGroupIndex], AssemblyResult.AssemblyGroup);
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.
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