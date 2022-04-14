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
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="NonAdoptableFailureMechanismSectionResult"/>.
    /// </summary>
    public class NonAdoptableFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<NonAdoptableFailureMechanismSectionResult>
    {
        private readonly int initialFailureMechanismResultTypeIndex;
        private readonly int initialFailureMechanismResultSectionProbabilityIndex;
        private readonly int furtherAnalysisTypeIndex;
        private readonly int refinedSectionProbabilityIndex;
        private readonly int sectionProbabilityIndex;
        private readonly int assemblyGroupIndex;

        private readonly IFailureMechanismSectionResultRowErrorProvider failureMechanismSectionResultRowErrorProvider;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="NonAdoptableFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="NonAdoptableFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="failureMechanismSectionResultRowErrorProvider">The error provider to use for
        /// the failure mechanism section result row.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="NonAdoptableFailureMechanismSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public NonAdoptableFailureMechanismSectionResultRow(NonAdoptableFailureMechanismSectionResult sectionResult,
                                                            IFailureMechanismSectionResultRowErrorProvider failureMechanismSectionResultRowErrorProvider,
                                                            IAssessmentSection assessmentSection,
                                                            ConstructionProperties constructionProperties)
            : base(sectionResult)
        {
            if (failureMechanismSectionResultRowErrorProvider == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResultRowErrorProvider));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            this.failureMechanismSectionResultRowErrorProvider = failureMechanismSectionResultRowErrorProvider;
            this.assessmentSection = assessmentSection;

            initialFailureMechanismResultTypeIndex = constructionProperties.InitialFailureMechanismResultTypeIndex;
            initialFailureMechanismResultSectionProbabilityIndex = constructionProperties.InitialFailureMechanismResultSectionProbabilityIndex;
            furtherAnalysisTypeIndex = constructionProperties.FurtherAnalysisTypeIndex;
            refinedSectionProbabilityIndex = constructionProperties.RefinedSectionProbabilityIndex;
            sectionProbabilityIndex = constructionProperties.SectionProbabilityIndex;
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
        /// Gets or sets the initial failure mechanism result type.
        /// </summary>
        public NonAdoptableInitialFailureMechanismResultType InitialFailureMechanismResultType
        {
            get => SectionResult.InitialFailureMechanismResultType;
            set
            {
                SectionResult.InitialFailureMechanismResultType = value;
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
            get => SectionResult.ManualInitialFailureMechanismResultSectionProbability;
            set
            {
                SectionResult.ManualInitialFailureMechanismResultSectionProbability = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the further analysis type.
        /// </summary>
        public FailureMechanismSectionResultFurtherAnalysisType FurtherAnalysisType
        {
            get => SectionResult.FurtherAnalysisType;
            set
            {
                SectionResult.FurtherAnalysisType = value;
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

        /// <summary>
        /// Gets the section probability.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double SectionProbability => AssemblyResult.SectionProbability;

        /// <summary>
        /// Gets the assembly group.
        /// </summary>
        public string AssemblyGroup => EnumDisplayNameHelper.GetDisplayName(AssemblyResult.FailureMechanismSectionAssemblyGroup);

        public override void Update()
        {
            UpdateAssemblyData();
            UpdateColumnStateDefinitions();
            UpdateInitialFailureMechanismResultErrors();
            UpdateRefinedFailureMechanismResultErrors();
        }

        private void UpdateAssemblyData()
        {
            ResetAssemblyResultErrorTexts();
            TryGetAssemblyResult();
        }

        private void UpdateInitialFailureMechanismResultErrors()
        {
            ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex].ErrorText = string.Empty;

            if (SectionResult.IsRelevant && SectionResult.InitialFailureMechanismResultType == NonAdoptableInitialFailureMechanismResultType.Manual)
            {
                ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex].ErrorText = failureMechanismSectionResultRowErrorProvider.GetManualProbabilityValidationError(
                    InitialFailureMechanismResultSectionProbability);
            }
        }

        private void UpdateRefinedFailureMechanismResultErrors()
        {
            ColumnStateDefinitions[refinedSectionProbabilityIndex].ErrorText = string.Empty;

            if (SectionResult.IsRelevant && SectionResult.FurtherAnalysisType == FailureMechanismSectionResultFurtherAnalysisType.Executed)
            {
                ColumnStateDefinitions[refinedSectionProbabilityIndex].ErrorText = failureMechanismSectionResultRowErrorProvider.GetManualProbabilityValidationError(RefinedSectionProbability);
            }
        }

        private void ResetAssemblyResultErrorTexts()
        {
            ColumnStateDefinitions[sectionProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[assemblyGroupIndex].ErrorText = string.Empty;
        }

        private void TryGetAssemblyResult()
        {
            try
            {
                AssemblyResult = FailureMechanismSectionAssemblyResultFactory.AssembleSection(SectionResult, assessmentSection).AssemblyResult;
            }
            catch (AssemblyException e)
            {
                AssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
                ColumnStateDefinitions[sectionProbabilityIndex].ErrorText = e.Message;
                ColumnStateDefinitions[assemblyGroupIndex].ErrorText = e.Message;
            }
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(initialFailureMechanismResultTypeIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(initialFailureMechanismResultSectionProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(furtherAnalysisTypeIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(refinedSectionProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(sectionProbabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(assemblyGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void UpdateColumnStateDefinitions()
        {
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[initialFailureMechanismResultTypeIndex], !IsRelevant);

            if (!IsRelevant || InitialFailureMechanismResultType == NonAdoptableInitialFailureMechanismResultType.NoFailureProbability)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex]);
            }

            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[furtherAnalysisTypeIndex], !IsRelevant);

            if (!IsRelevant || FurtherAnalysisType != FailureMechanismSectionResultFurtherAnalysisType.Executed)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[refinedSectionProbabilityIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[refinedSectionProbabilityIndex]);
            }

            FailureMechanismSectionResultRowHelper.SetAssemblyGroupStyle(ColumnStateDefinitions[assemblyGroupIndex], AssemblyResult.FailureMechanismSectionAssemblyGroup);
        }

        public class ConstructionProperties
        {
            /// <summary>
            /// Sets the initial failure mechanism result type index.
            /// </summary>
            public int InitialFailureMechanismResultTypeIndex { internal get; set; }

            /// <summary>
            /// Sets the initial failure mechanism result section probability index.
            /// </summary>
            public int InitialFailureMechanismResultSectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the further analysis type index.
            /// </summary>
            public int FurtherAnalysisTypeIndex { internal get; set; }

            /// <summary>
            /// Sets the refined section probability index.
            /// </summary>
            public int RefinedSectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the section probability index.
            /// </summary>
            public int SectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the assembly group index.
            /// </summary>
            public int AssemblyGroupIndex { internal get; set; }
        }
    }
}