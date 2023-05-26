// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Core.Common.Util.Enums;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Primitives;
using CommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.
    /// </summary>
    public class NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        private readonly int initialFailureMechanismResultTypeIndex;
        private readonly int initialFailureMechanismResultProfileProbabilityIndex;
        private readonly int initialFailureMechanismResultSectionProbabilityIndex;
        private readonly int furtherAnalysisTypeIndex;
        private readonly int refinedProfileProbabilityIndex;
        private readonly int refinedSectionProbabilityIndex;
        private readonly int profileProbabilityIndex;
        private readonly int sectionProbabilityIndex;
        private readonly int sectionNIndex;
        private readonly int assemblyGroupIndex;

        private readonly IFailureMechanismSectionResultRowErrorProvider failureMechanismSectionResultRowErrorProvider;
        private readonly Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc;

        /// <summary>
        /// Creates a new instance of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="failureMechanismSectionResultRowErrorProvider">The error provider to use for
        /// the failure mechanism section result row.</param>
        /// <param name="performAssemblyFunc">Function to perform the assembly.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        public NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                                  IFailureMechanismSectionResultRowErrorProvider failureMechanismSectionResultRowErrorProvider,
                                                                                  Func<FailureMechanismSectionAssemblyResultWrapper> performAssemblyFunc,
                                                                                  ConstructionProperties constructionProperties)
            : base(sectionResult)
        {
            if (failureMechanismSectionResultRowErrorProvider == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResultRowErrorProvider));
            }

            if (performAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performAssemblyFunc));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            this.failureMechanismSectionResultRowErrorProvider = failureMechanismSectionResultRowErrorProvider;
            this.performAssemblyFunc = performAssemblyFunc;

            initialFailureMechanismResultTypeIndex = constructionProperties.InitialFailureMechanismResultTypeIndex;
            initialFailureMechanismResultProfileProbabilityIndex = constructionProperties.InitialFailureMechanismResultProfileProbabilityIndex;
            initialFailureMechanismResultSectionProbabilityIndex = constructionProperties.InitialFailureMechanismResultSectionProbabilityIndex;
            furtherAnalysisTypeIndex = constructionProperties.FurtherAnalysisTypeIndex;
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
        /// Gets or sets the value of the initial failure mechanism result per profile as a probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not in range [0,1].</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double InitialFailureMechanismResultProfileProbability
        {
            get => SectionResult.ManualInitialFailureMechanismResultProfileProbability;
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
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble SectionN => new RoundedDouble(2, AssemblyResult.N);

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

        private void UpdateInitialFailureMechanismResultErrors()
        {
            ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex].ErrorText = string.Empty;

            if (SectionResult.IsRelevant && SectionResult.InitialFailureMechanismResultType == NonAdoptableInitialFailureMechanismResultType.Manual)
            {
                ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex].ErrorText = failureMechanismSectionResultRowErrorProvider.GetManualProbabilityValidationError(
                    InitialFailureMechanismResultProfileProbability);
                ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex].ErrorText = failureMechanismSectionResultRowErrorProvider.GetManualProbabilityValidationError(
                    InitialFailureMechanismResultSectionProbability);
            }
        }

        private void UpdateRefinedFailureMechanismResultErrors()
        {
            ColumnStateDefinitions[refinedProfileProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[refinedSectionProbabilityIndex].ErrorText = string.Empty;

            if (SectionResult.IsRelevant && SectionResult.FurtherAnalysisType == FailureMechanismSectionResultFurtherAnalysisType.Executed)
            {
                ColumnStateDefinitions[refinedProfileProbabilityIndex].ErrorText = failureMechanismSectionResultRowErrorProvider.GetManualProbabilityValidationError(
                    RefinedProfileProbability);
                ColumnStateDefinitions[refinedSectionProbabilityIndex].ErrorText = failureMechanismSectionResultRowErrorProvider.GetManualProbabilityValidationError(
                    RefinedSectionProbability);
            }
        }

        private void UpdateAssemblyData()
        {
            ResetAssemblyResultErrorTexts();
            TryGetAssemblyResult();
        }

        private void ResetAssemblyResultErrorTexts()
        {
            ColumnStateDefinitions[profileProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[sectionProbabilityIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[sectionNIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[assemblyGroupIndex].ErrorText = string.Empty;
        }

        private void TryGetAssemblyResult()
        {
            try
            {
                AssemblyResult = performAssemblyFunc().AssemblyResult;
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
            ColumnStateDefinitions.Add(initialFailureMechanismResultTypeIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(initialFailureMechanismResultProfileProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(initialFailureMechanismResultSectionProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(furtherAnalysisTypeIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(refinedProfileProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(refinedSectionProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(profileProbabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(sectionProbabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(sectionNIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(assemblyGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void UpdateColumnStateDefinitions()
        {
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[initialFailureMechanismResultTypeIndex], !IsRelevant);

            if (!IsRelevant || InitialFailureMechanismResultType == NonAdoptableInitialFailureMechanismResultType.NoFailureProbability)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex]);
            }

            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[furtherAnalysisTypeIndex], !IsRelevant);

            if (!IsRelevant || FurtherAnalysisType != FailureMechanismSectionResultFurtherAnalysisType.Executed)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[refinedProfileProbabilityIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[refinedSectionProbabilityIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[refinedProfileProbabilityIndex]);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[refinedSectionProbabilityIndex]);
            }

            FailureMechanismSectionResultRowHelper.SetAssemblyGroupStyle(ColumnStateDefinitions[assemblyGroupIndex], AssemblyResult.FailureMechanismSectionAssemblyGroup);
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Sets the initial failure mechanism result type index.
            /// </summary>
            public int InitialFailureMechanismResultTypeIndex { internal get; set; }

            /// <summary>
            /// Sets the initial failure mechanism result profile probability index.
            /// </summary>
            public int InitialFailureMechanismResultProfileProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the initial failure mechanism result section probability index.
            /// </summary>
            public int InitialFailureMechanismResultSectionProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the further analysis type index.
            /// </summary>
            public int FurtherAnalysisTypeIndex { internal get; set; }

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