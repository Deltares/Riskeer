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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Controls.DataGrid;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Forms.Helpers;
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
        private readonly int simpleAssessmentResultIndex;
        private readonly int detailedAssessmentResultIndex;
        private readonly int detailedAssessmentProbabilityIndex;
        private readonly int tailorMadeAssessmentResultIndex;
        private readonly int tailorMadeAssessmentProbabilityIndex;
        private readonly int simpleAssemblyCategoryGroupIndex;
        private readonly int detailedAssemblyCategoryGroupIndex;
        private readonly int tailorMadeAssemblyCategoryGroupIndex;
        private readonly int combinedAssemblyCategoryGroupIndex;
        private readonly int manualAssemblyCategoryGroupIndex;

        private readonly MacroStabilityOutwardsFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityOutwardsSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="MacroStabilityOutwardsSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityOutwardsSectionResultRow(MacroStabilityOutwardsFailureMechanismSectionResult sectionResult,
                                                      MacroStabilityOutwardsFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection,
                                                      ConstructionProperties constructionProperties)
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

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

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
            manualAssemblyCategoryGroupIndex = constructionProperties.ManualAssemblyCategoryGroupIndex;

            ColumnStateDefinitions = CreateColumnStateDefinitions();

            Update();
        }

        /// <summary>
        /// Gets the column state definitions for the given indices.
        /// </summary>
        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

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
                Update();
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
                Update();
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
                Update();
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets the value representing the tailor made assessment result.
        /// </summary>
        public TailorMadeAssessmentProbabilityAndDetailedCalculationResultType TailorMadeAssessmentResult
        {
            get
            {
                return SectionResult.TailorMadeAssessmentResult;
            }
            set
            {
                SectionResult.TailorMadeAssessmentResult = value;
                Update();
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
                Update();
                SectionResult.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets the simple assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup SimpleAssemblyCategoryGroup { get; private set; }

        /// <summary>
        /// Gets the detailed assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup DetailedAssemblyCategoryGroup { get; private set; }

        /// <summary>
        /// Gets the tailor made assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup TailorMadeAssemblyCategoryGroup { get; private set; }

        /// <summary>
        /// Gets the combined assembly category group.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedAssemblyCategoryGroup { get; private set; }

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
                Update();
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
                Update();
                SectionResult.NotifyObservers();
            }
        }

        private Dictionary<int, DataGridViewColumnStateDefinition> CreateColumnStateDefinitions()
        {
            return new Dictionary<int, DataGridViewColumnStateDefinition>
            {
                {
                    simpleAssessmentResultIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    detailedAssessmentResultIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    detailedAssessmentProbabilityIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    tailorMadeAssessmentResultIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    tailorMadeAssessmentProbabilityIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    simpleAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    detailedAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    tailorMadeAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    combinedAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition()
                },
                {
                    manualAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition()
                }
            };
        }

        /// <summary>
        /// Updates the derived assembly category groups.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        private void Update()
        {
            UpdateData();
            UpdateColumnDefinitionStates();
        }

        private void UpdateData()
        {
            SimpleAssemblyCategoryGroup = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(SectionResult);

            DetailedAssemblyCategoryGroup = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                SectionResult,
                failureMechanism,
                assessmentSection);

            TailorMadeAssemblyCategoryGroup = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                SectionResult,
                failureMechanism,
                assessmentSection);

            CombinedAssemblyCategoryGroup = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                SectionResult,
                failureMechanism,
                assessmentSection);
        }

        private void UpdateColumnDefinitionStates()
        {
            bool simpleAssessmentSufficient = GetSimpleAssessmentSufficient();

            SetColumnState(detailedAssessmentResultIndex, simpleAssessmentSufficient || UseManualAssemblyCategoryGroup);
            SetColumnState(detailedAssessmentProbabilityIndex, simpleAssessmentSufficient || GetDetailedAssessmentResultIsNotProbability() || UseManualAssemblyCategoryGroup);
            SetColumnState(tailorMadeAssessmentResultIndex, simpleAssessmentSufficient || UseManualAssemblyCategoryGroup);
            SetColumnState(tailorMadeAssessmentProbabilityIndex, simpleAssessmentSufficient || GetTailorMadeAssessmentResultIsNotProbability() || UseManualAssemblyCategoryGroup);

            if (UseManualAssemblyCategoryGroup)
            {
                DisableColumn(simpleAssemblyCategoryGroupIndex);
                DisableColumn(detailedAssemblyCategoryGroupIndex);
                DisableColumn(tailorMadeAssemblyCategoryGroupIndex);
                DisableColumn(combinedAssemblyCategoryGroupIndex);
            }

            SetColumnState(manualAssemblyCategoryGroupIndex, !UseManualAssemblyCategoryGroup);
        }

        private void SetColumnState(int columnIndex, bool shouldDisable)
        {
            if (shouldDisable)
            {
                DisableColumn(columnIndex);
            }
            else
            {
                EnableColumn(columnIndex);
            }
        }

        private void EnableColumn(int index)
        {
            ColumnStateDefinitions[index].ReadOnly = false;
            ColumnStateDefinitions[index].Style = CellStyle.Enabled;
        }

        private void DisableColumn(int index)
        {
            ColumnStateDefinitions[index].ReadOnly = true;
            ColumnStateDefinitions[index].Style = CellStyle.Disabled;
        }

        private bool GetSimpleAssessmentSufficient()
        {
            return FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(SimpleAssessmentResult);
        }

        private bool GetDetailedAssessmentResultIsNotProbability()
        {
            return DetailedAssessmentResult != DetailedAssessmentResultType.Probability;
        }

        private bool GetTailorMadeAssessmentResultIsNotProbability()
        {
            return TailorMadeAssessmentResult != TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability;
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="MacroStabilityOutwardsSectionResultRow"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the simple assessment result index.
            /// </summary>
            public int SimpleAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the detailed assessment result index.
            /// </summary>
            public int DetailedAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the detailed assessment probability index.
            /// </summary>
            public int DetailedAssessmentProbabilityIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the tailor made assessment result index.
            /// </summary>
            public int TailorMadeAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the tailor made assessment probability index.
            /// </summary>
            public int TailorMadeAssessmentProbabilityIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the simple assembly category group index.
            /// </summary>
            public int SimpleAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the detailed assembly category group index.
            /// </summary>
            public int DetailedAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the tailor made assembly category group index.
            /// </summary>
            public int TailorMadeAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the combined assembly category group index.
            /// </summary>
            public int CombinedAssemblyCategoryGroupIndex { internal get; set; }

            /// <summary>
            /// Gets or sets the manual assembly category group index.
            /// </summary>
            public int ManualAssemblyCategoryGroupIndex { internal get; set; }
        }
    }
}