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
using System.Linq;
using Core.Common.Controls.DataGrid;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="PipingFailureMechanismSectionResultOld"/>.
    /// </summary>
    public class PipingFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<PipingFailureMechanismSectionResultOld>
    {
        private readonly int simpleAssessmentResultIndex;
        private readonly int detailedAssessmentResultIndex;
        private readonly int detailedAssessmentProbabilityIndex;
        private readonly int tailorMadeAssessmentResultIndex;
        private readonly int tailorMadeAssessmentProbabilityIndex;
        private readonly int simpleAssemblyCategoryGroupIndex;
        private readonly int detailedAssemblyCategoryGroupIndex;
        private readonly int tailorMadeAssemblyCategoryGroupIndex;
        private readonly int combinedAssemblyCategoryGroupIndex;
        private readonly int combinedAssemblyProbabilityIndex;
        private readonly int manualAssemblyProbabilityIndex;

        private readonly IEnumerable<SemiProbabilisticPipingCalculationScenario> calculations;
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;
        private FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup combinedAssemblyCategoryGroup;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="PipingFailureMechanismSectionResultOld"/> that is 
        /// the source of this row.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="PipingFailureMechanismSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Throw when any parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        internal PipingFailureMechanismSectionResultRow(PipingFailureMechanismSectionResultOld sectionResult,
                                                        IEnumerable<SemiProbabilisticPipingCalculationScenario> calculations,
                                                        PipingFailureMechanism failureMechanism,
                                                        IAssessmentSection assessmentSection,
                                                        ConstructionProperties constructionProperties)
            : base(sectionResult)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
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

            this.calculations = calculations;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;

            simpleAssessmentResultIndex = constructionProperties.SimpleAssessmentResultIndex;
            detailedAssessmentResultIndex = constructionProperties.DetailedAssessmentResultIndex;
            detailedAssessmentProbabilityIndex = constructionProperties.DetailedAssessmentProbabilityIndex;
            tailorMadeAssessmentResultIndex = constructionProperties.TailorMadeAssessmentResultIndex;
            tailorMadeAssessmentProbabilityIndex = constructionProperties.TailorMadeAssessmentProbabilityIndex;
            simpleAssemblyCategoryGroupIndex = constructionProperties.SimpleAssemblyCategoryGroupIndex;
            detailedAssemblyCategoryGroupIndex = constructionProperties.DetailedAssemblyCategoryGroupIndex;
            tailorMadeAssemblyCategoryGroupIndex = constructionProperties.TailorMadeAssemblyCategoryGroupIndex;
            combinedAssemblyCategoryGroupIndex = constructionProperties.CombinedAssemblyCategoryGroupIndex;
            combinedAssemblyProbabilityIndex = constructionProperties.CombinedAssemblyProbabilityIndex;
            manualAssemblyProbabilityIndex = constructionProperties.ManualAssemblyProbabilityIndex;

            CreateColumnStateDefinitions();

            Update();
        }

        /// <summary>
        /// Gets or sets the value representing the simple assessment result.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public SimpleAssessmentResultType SimpleAssessmentResult
        {
            get => SectionResult.SimpleAssessmentResult;
            set
            {
                SectionResult.SimpleAssessmentResult = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value representing the detailed assessment result.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public DetailedAssessmentProbabilityOnlyResultType DetailedAssessmentResult
        {
            get => SectionResult.DetailedAssessmentResult;
            set
            {
                SectionResult.DetailedAssessmentResult = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets the detailed assessment probability a of the <see cref="PipingFailureMechanismSectionResultOld"/>.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double DetailedAssessmentProbability => SectionResult.GetDetailedAssessmentProbability(calculations, assessmentSection.FailureMechanismContribution.Norm);

        /// <summary>
        /// Gets or sets the value representing the tailor made assessment result.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public TailorMadeAssessmentProbabilityCalculationResultType TailorMadeAssessmentResult
        {
            get => SectionResult.TailorMadeAssessmentResult;
            set
            {
                SectionResult.TailorMadeAssessmentResult = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is 
        /// not in the range [0,1].</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TailorMadeAssessmentProbability
        {
            get => SectionResult.TailorMadeAssessmentProbability;
            set
            {
                SectionResult.TailorMadeAssessmentProbability = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets the simple assembly category group.
        /// </summary>
        public string SimpleAssemblyCategoryGroup => FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(simpleAssemblyCategoryGroup);

        /// <summary>
        /// Gets the detailed assembly category group.
        /// </summary>
        public string DetailedAssemblyCategoryGroup => FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(detailedAssemblyCategoryGroup);

        /// <summary>
        /// Gets the tailor made assembly category group.
        /// </summary>
        public string TailorMadeAssemblyCategoryGroup => FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(tailorMadeAssemblyCategoryGroup);

        /// <summary>
        /// Gets the combined assembly category group.
        /// </summary>
        public string CombinedAssemblyCategoryGroup => FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedAssemblyCategoryGroup);

        /// <summary>
        /// Gets the combined assembly probability.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CombinedAssemblyProbability { get; private set; }

        /// <summary>
        /// Gets or sets the indicator whether the combined assembly probability
        /// should be overwritten by <see cref="ManualAssemblyProbability"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public bool UseManualAssembly
        {
            get => SectionResult.UseManualAssembly;
            set
            {
                SectionResult.UseManualAssembly = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the manually entered assembly probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is 
        /// not in the range [0,1].</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double ManualAssemblyProbability
        {
            get => SectionResult.ManualAssemblyProbability;
            set
            {
                SectionResult.ManualAssemblyProbability = value;
                UpdateInternalData();
            }
        }

        public override void Update()
        {
            UpdateDerivedData();
            UpdateColumnStateDefinitions();
            UpdateDetailedAssessmentProbabilityError();
        }

        private void UpdateDetailedAssessmentProbabilityError()
        {
            if (FailureMechanismSectionResultRowHelper.SimpleAssessmentIsSufficient(SimpleAssessmentResult)
                || !FailureMechanismSectionResultRowHelper.DetailedAssessmentResultIsProbability(DetailedAssessmentResult)
                || UseManualAssembly)
            {
                ColumnStateDefinitions[detailedAssessmentProbabilityIndex].ErrorText = string.Empty;
            }
            else
            {
                ColumnStateDefinitions[detailedAssessmentProbabilityIndex].ErrorText = FailureMechanismSectionResultRowHelper.GetDetailedAssessmentProbabilityError(
                    SectionResult.GetCalculationScenarios(calculations).ToArray(),
                    scenarios => SectionResult.GetTotalContribution(scenarios),
                    scenarios => SectionResult.GetDetailedAssessmentProbability(scenarios, assessmentSection.FailureMechanismContribution.Norm));
            }
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(simpleAssessmentResultIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentResultIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(tailorMadeAssessmentResultIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(tailorMadeAssessmentProbabilityIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(simpleAssemblyCategoryGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssemblyCategoryGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(tailorMadeAssemblyCategoryGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(combinedAssemblyCategoryGroupIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(combinedAssemblyProbabilityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(manualAssemblyProbabilityIndex, new DataGridViewColumnStateDefinition());
        }

        private void UpdateDerivedData()
        {
            ResetErrorTexts();
            TryGetSimpleAssemblyCategoryGroup();
            TryGetDetailedAssemblyCategoryGroup();
            TryGetTailorMadeAssemblyCategoryGroup();
            TryGetCombinedAssemblyCategoryGroup();
        }

        private void ResetErrorTexts()
        {
            ColumnStateDefinitions[simpleAssemblyCategoryGroupIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[detailedAssemblyCategoryGroupIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[tailorMadeAssemblyCategoryGroupIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[combinedAssemblyCategoryGroupIndex].ErrorText = string.Empty;
            ColumnStateDefinitions[combinedAssemblyProbabilityIndex].ErrorText = string.Empty;
        }

        private void TryGetSimpleAssemblyCategoryGroup()
        {
            try
            {
                simpleAssemblyCategoryGroup = PipingFailureMechanismAssemblyFactory.AssembleSimpleAssessment(SectionResult).Group;
            }
            catch (AssemblyException e)
            {
                simpleAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
                ColumnStateDefinitions[simpleAssemblyCategoryGroupIndex].ErrorText = e.Message;
            }
        }

        private void TryGetDetailedAssemblyCategoryGroup()
        {
            try
            {
                detailedAssemblyCategoryGroup = PipingFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    SectionResult,
                    calculations,
                    failureMechanism,
                    assessmentSection).Group;
            }
            catch (AssemblyException e)
            {
                detailedAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
                ColumnStateDefinitions[detailedAssemblyCategoryGroupIndex].ErrorText = e.Message;
            }
        }

        private void TryGetTailorMadeAssemblyCategoryGroup()
        {
            try
            {
                tailorMadeAssemblyCategoryGroup = PipingFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    SectionResult,
                    failureMechanism,
                    assessmentSection).Group;
            }
            catch (AssemblyException e)
            {
                tailorMadeAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
                ColumnStateDefinitions[tailorMadeAssemblyCategoryGroupIndex].ErrorText = e.Message;
            }
        }

        private void TryGetCombinedAssemblyCategoryGroup()
        {
            try
            {
                FailureMechanismSectionAssembly combinedAssembly =
                    PipingFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                        SectionResult,
                        calculations,
                        failureMechanism,
                        assessmentSection);

                combinedAssemblyCategoryGroup = combinedAssembly.Group;
                CombinedAssemblyProbability = combinedAssembly.Probability;
            }
            catch (AssemblyException e)
            {
                combinedAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
                CombinedAssemblyProbability = double.NaN;
                ColumnStateDefinitions[combinedAssemblyCategoryGroupIndex].ErrorText = e.Message;
                ColumnStateDefinitions[combinedAssemblyProbabilityIndex].ErrorText = e.Message;
            }
        }

        /// <summary>
        /// Updates the column state definitions.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        private void UpdateColumnStateDefinitions()
        {
            bool simpleAssessmentSufficient = FailureMechanismSectionResultRowHelper.SimpleAssessmentIsSufficient(SimpleAssessmentResult);

            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[simpleAssessmentResultIndex], UseManualAssembly);
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[detailedAssessmentResultIndex], simpleAssessmentSufficient
                                                                                                    || UseManualAssembly);
            if (simpleAssessmentSufficient
                || !FailureMechanismSectionResultRowHelper.DetailedAssessmentResultIsProbability(DetailedAssessmentResult)
                || UseManualAssembly)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[detailedAssessmentProbabilityIndex]);
            }
            else
            {
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[detailedAssessmentProbabilityIndex], true);
            }

            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[tailorMadeAssessmentResultIndex],
                                             simpleAssessmentSufficient
                                             || UseManualAssembly);
            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[tailorMadeAssessmentProbabilityIndex],
                                             simpleAssessmentSufficient
                                             || !FailureMechanismSectionResultRowHelper.TailorMadeAssessmentResultIsProbability(TailorMadeAssessmentResult)
                                             || UseManualAssembly);

            if (UseManualAssembly)
            {
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[simpleAssemblyCategoryGroupIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[detailedAssemblyCategoryGroupIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[tailorMadeAssemblyCategoryGroupIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[combinedAssemblyCategoryGroupIndex]);
                ColumnStateHelper.DisableColumn(ColumnStateDefinitions[combinedAssemblyProbabilityIndex]);
            }
            else
            {
                FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(ColumnStateDefinitions[simpleAssemblyCategoryGroupIndex],
                                                                                     simpleAssemblyCategoryGroup);
                FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(ColumnStateDefinitions[detailedAssemblyCategoryGroupIndex],
                                                                                     detailedAssemblyCategoryGroup);
                FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(ColumnStateDefinitions[tailorMadeAssemblyCategoryGroupIndex],
                                                                                     tailorMadeAssemblyCategoryGroup);
                FailureMechanismSectionResultRowHelper.SetAssemblyCategoryGroupStyle(ColumnStateDefinitions[combinedAssemblyCategoryGroupIndex],
                                                                                     combinedAssemblyCategoryGroup);
                ColumnStateHelper.EnableColumn(ColumnStateDefinitions[combinedAssemblyProbabilityIndex], true);
            }

            ColumnStateHelper.SetColumnState(ColumnStateDefinitions[manualAssemblyProbabilityIndex], !UseManualAssembly);
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="PipingFailureMechanismSectionResultRow"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Sets the simple assessment result index.
            /// </summary>
            public int SimpleAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment result index.
            /// </summary>
            public int DetailedAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment probability index.
            /// </summary>
            public int DetailedAssessmentProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the tailor made assessment result index.
            /// </summary>
            public int TailorMadeAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Sets the tailor made assessment probability index.
            /// </summary>
            public int TailorMadeAssessmentProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the simple assembly category group index.
            /// </summary>
            public int SimpleAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assembly category group index.
            /// </summary>
            public int DetailedAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Sets the tailor made assembly category group index.
            /// </summary>
            public int TailorMadeAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Sets the combined assembly category group index.
            /// </summary>
            public int CombinedAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Sets the combined assembly probability index.
            /// </summary>
            public int CombinedAssemblyProbabilityIndex { internal get; set; }

            /// <summary>
            /// Sets the manual assembly category group index.
            /// </summary>
            public int ManualAssemblyProbabilityIndex { internal get; set; }
        }
    }
}