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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Properties;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public class PipingFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<PipingFailureMechanismSectionResult>
    {
        private readonly int initialFailureMechanismResultIndex;
        private readonly int initialFailureMechanismResultProfileProbabilityIndex;
        private readonly int initialFailureMechanismResultSectionProbabilityIndex;
        private readonly int furtherAnalysisNeededIndex;
        private readonly int probabilityRefinementTypeIndex;
        private readonly int refinedProfileProbabilityIndex;
        private readonly int refinedSectionProbabilityIndex;

        private readonly IPipingFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy;
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private FailureMechanismSectionAssemblyGroup assemblyGroup;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="PipingFailureMechanismSectionResult"/> that is 
        /// the source of this row.</param>
        /// <param name="calculateProbabilityStrategy">The strategy used to calculate probabilities.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="PipingFailureMechanismSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        internal PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResult sectionResult,
                                                        IPipingFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy,
                                                        PipingFailureMechanism failureMechanism,
                                                        IAssessmentSection assessmentSection,
                                                        ConstructionProperties constructionProperties)
            : base(sectionResult)
        {
            if (calculateProbabilityStrategy == null)
            {
                throw new ArgumentNullException(nameof(calculateProbabilityStrategy));
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

            this.calculateProbabilityStrategy = calculateProbabilityStrategy;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            initialFailureMechanismResultIndex = constructionProperties.InitialFailureMechanismResultIndex;
            initialFailureMechanismResultProfileProbabilityIndex = constructionProperties.InitialFailureMechanismResultProfileProbabilityIndex;
            initialFailureMechanismResultSectionProbabilityIndex = constructionProperties.InitialFailureMechanismResultSectionProbabilityIndex;
            furtherAnalysisNeededIndex = constructionProperties.FurtherAnalysisNeededIndex;
            probabilityRefinementTypeIndex = constructionProperties.ProbabilityRefinementTypeIndex;
            refinedProfileProbabilityIndex = constructionProperties.RefinedProfileProbabilityIndex;
            refinedSectionProbabilityIndex = constructionProperties.RefinedSectionProbabilityIndex;

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
        public object RefinedProfileProbability
        {
            get => ProbabilityRefinementType == ProbabilityRefinementType.Section
                       ? (object) Resources.PipingFailureMechanismSectionResultRow_Derived_DisplayName
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
                       ? (object) Resources.PipingFailureMechanismSectionResultRow_Derived_DisplayName
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
        public double ProfileProbability { get; private set; }

        /// <summary>
        /// Gets the section probability.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double SectionProbability { get; private set; }

        /// <summary>
        /// Gets the resulting section N.
        /// </summary>
        public double ResultingSectionN { get; private set; }

        /// <summary>
        /// Gets the assembly group.
        /// </summary>
        public string AssemblyGroup => assemblyGroup.ToString();

        public override void Update()
        {
            UpdateDerivedData();
            UpdateColumnStateDefinitions();
        }

        private void UpdateDerivedData()
        {
            double refinedProfileProbability = SectionResult.RefinedProfileProbability;
            double refinedSectionProbability = SectionResult.RefinedSectionProbability;
            double sectionN = failureMechanism.PipingProbabilityAssessmentInput.GetN(SectionResult.Section.Length);

            if (ProbabilityRefinementType == ProbabilityRefinementType.Profile)
            {
                refinedSectionProbability = SectionResult.RefinedProfileProbability * sectionN;
            }

            if (ProbabilityRefinementType == ProbabilityRefinementType.Section)
            {
                refinedProfileProbability = SectionResult.RefinedSectionProbability / sectionN;
            }

            try
            {
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                    assessmentSection, IsRelevant, InitialFailureMechanismResultProfileProbability,
                    initialFailureMechanismResultSectionProbabilityIndex, FurtherAnalysisNeeded, refinedProfileProbability, refinedSectionProbability);
                ProfileProbability = result.ProfileProbability;
                SectionProbability = result.SectionProbability;
                ResultingSectionN = result.N;
                assemblyGroup = result.AssemblyGroup;
            }
            catch (AssemblyException e)
            {
                ProfileProbability = double.NaN;
                SectionProbability = double.NaN;
                ResultingSectionN = double.NaN;
                assemblyGroup = FailureMechanismSectionAssemblyGroup.D;
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
        }

        private void UpdateColumnStateDefinitions()
        {
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[initialFailureMechanismResultIndex], !IsRelevant);

            if (!IsRelevant || InitialFailureMechanismResult == InitialFailureMechanismResultType.NoFailureProbability)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultProfileProbabilityIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[initialFailureMechanismResultSectionProbabilityIndex]);
            }
            else
            {
                bool initialFailureMechanismResultAdopt = InitialFailureMechanismResult == InitialFailureMechanismResultType.Adopt;
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
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="PipingFailureMechanismSectionResultRow"/>.
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
        }
    }
}