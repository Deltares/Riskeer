﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.Views
{
    /// <summary>
    /// Class for displaying <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>
    /// as a row in a grid view.
    /// </summary>
    public class StabilityStoneCoverSectionResultRow : FailureMechanismSectionResultRow<StabilityStoneCoverFailureMechanismSectionResult>
    {
        private readonly int simpleAssessmentResultIndex;
        private readonly int detailedAssessmentResultForFactorizedSignalingNormIndex;
        private readonly int detailedAssessmentResultForSignalingNormIndex;
        private readonly int detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex;
        private readonly int detailedAssessmentResultForLowerLimitNormIndex;
        private readonly int detailedAssessmentResultForFactorizedLowerLimitNormIndex;
        private readonly int tailorMadeAssessmentResultIndex;
        private readonly int simpleAssemblyCategoryGroupIndex;
        private readonly int detailedAssemblyCategoryGroupIndex;
        private readonly int tailorMadeAssemblyCategoryGroupIndex;
        private readonly int combinedAssemblyCategoryGroupIndex;
        private readonly int manualAssemblyCategoryGroupIndex;

        private FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup combinedAssemblyCategoryGroup;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>
        /// to wrap so that it can be displayed as a row.</param>
        /// <param name="constructionProperties">The property values required to create an instance of
        /// <see cref="StabilityStoneCoverSectionResultRow"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityStoneCoverSectionResultRow(StabilityStoneCoverFailureMechanismSectionResult sectionResult,
                                                   ConstructionProperties constructionProperties)
            : base(sectionResult)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            simpleAssessmentResultIndex = constructionProperties.SimpleAssessmentResultIndex;
            detailedAssessmentResultForFactorizedSignalingNormIndex = constructionProperties.DetailedAssessmentResultForFactorizedSignalingNormIndex;
            detailedAssessmentResultForSignalingNormIndex = constructionProperties.DetailedAssessmentResultForSignalingNormIndex;
            detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = constructionProperties.DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex;
            detailedAssessmentResultForLowerLimitNormIndex = constructionProperties.DetailedAssessmentResultForLowerLimitNormIndex;
            detailedAssessmentResultForFactorizedLowerLimitNormIndex = constructionProperties.DetailedAssessmentResultForFactorizedLowerLimitNormIndex;
            tailorMadeAssessmentResultIndex = constructionProperties.TailorMadeAssessmentResultIndex;
            simpleAssemblyCategoryGroupIndex = constructionProperties.SimpleAssemblyCategoryGroupIndex;
            detailedAssemblyCategoryGroupIndex = constructionProperties.DetailedAssemblyCategoryGroupIndex;
            tailorMadeAssemblyCategoryGroupIndex = constructionProperties.TailorMadeAssemblyCategoryGroupIndex;
            combinedAssemblyCategoryGroupIndex = constructionProperties.CombinedAssemblyCategoryGroupIndex;
            manualAssemblyCategoryGroupIndex = constructionProperties.ManualAssemblyCategoryGroupIndex;

            CreateColumnStateDefinitions();

            Update();
        }

        /// <summary>
        /// Gets or sets the value representing the simple assessment result.
        /// </summary>
        public SimpleAssessmentValidityOnlyResultType SimpleAssessmentResult
        {
            get
            {
                return SectionResult.SimpleAssessmentResult;
            }
            set
            {
                SectionResult.SimpleAssessmentResult = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section
        /// for the factorized signaling norm (Cat Iv - IIv).
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForFactorizedSignalingNorm
        {
            get
            {
                return SectionResult.DetailedAssessmentResultForFactorizedSignalingNorm;
            }
            set
            {
                SectionResult.DetailedAssessmentResultForFactorizedSignalingNorm = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section
        /// for the signaling norm (Cat IIv - IIIv).
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForSignalingNorm
        {
            get
            {
                return SectionResult.DetailedAssessmentResultForSignalingNorm;
            }
            set
            {
                SectionResult.DetailedAssessmentResultForSignalingNorm = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section
        /// for the signaling norm (Cat IIIv - IVv).
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForMechanismSpecificLowerLimitNorm
        {
            get
            {
                return SectionResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm;
            }
            set
            {
                SectionResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section
        /// for the signaling norm (Cat IVv - Vv).
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForLowerLimitNorm
        {
            get
            {
                return SectionResult.DetailedAssessmentResultForLowerLimitNorm;
            }
            set
            {
                SectionResult.DetailedAssessmentResultForLowerLimitNorm = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section
        /// for the signaling norm (Cat Vv - VIv).
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForFactorizedLowerLimitNorm
        {
            get
            {
                return SectionResult.DetailedAssessmentResultForFactorizedLowerLimitNorm;
            }
            set
            {
                SectionResult.DetailedAssessmentResultForFactorizedLowerLimitNorm = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the tailor made assessment result.
        /// </summary>
        public SelectableFailureMechanismSectionAssemblyCategoryGroup TailorMadeAssessmentResult
        {
            get
            {
                return SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(SectionResult.TailorMadeAssessmentResult);
            }
            set
            {
                SectionResult.TailorMadeAssessmentResult = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(value);
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets the simple assembly category group.
        /// </summary>
        public string SimpleAssemblyCategoryGroup
        {
            get
            {
                return FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(simpleAssemblyCategoryGroup);
            }
        }

        /// <summary>
        /// Gets the detailed assembly category group.
        /// </summary>
        public string DetailedAssemblyCategoryGroup
        {
            get
            {
                return FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(detailedAssemblyCategoryGroup);
            }
        }

        /// <summary>
        /// Gets the tailor made assembly category group.
        /// </summary>
        public string TailorMadeAssemblyCategoryGroup
        {
            get
            {
                return FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(tailorMadeAssemblyCategoryGroup);
            }
        }

        /// <summary>
        /// Gets the combined assembly category group.
        /// </summary>
        public string CombinedAssemblyCategoryGroup
        {
            get
            {
                return FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(combinedAssemblyCategoryGroup);
            }
        }

        /// <summary>
        /// Gets or sets the indicator whether the combined assembly should be overwritten by <see cref="ManualAssemblyCategoryGroup"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public bool UseManualAssemblyCategoryGroup
        {
            get
            {
                return SectionResult.UseManualAssemblyCategoryGroup;
            }
            set
            {
                SectionResult.UseManualAssemblyCategoryGroup = value;
                UpdateInternalData();
            }
        }

        /// <summary>
        /// Gets or sets the manually selected assembly category group.
        /// </summary>
        public SelectableFailureMechanismSectionAssemblyCategoryGroup ManualAssemblyCategoryGroup
        {
            get
            {
                return SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(SectionResult.ManualAssemblyCategoryGroup);
            }
            set
            {
                SectionResult.ManualAssemblyCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(value);
                UpdateInternalData();
            }
        }

        public override void Update()
        {
            UpdateDerivedData();
        }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(simpleAssessmentResultIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentResultForFactorizedSignalingNormIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentResultForSignalingNormIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentResultForLowerLimitNormIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(detailedAssessmentResultForFactorizedLowerLimitNormIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(tailorMadeAssessmentResultIndex, new DataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(simpleAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            });
            ColumnStateDefinitions.Add(detailedAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            });
            ColumnStateDefinitions.Add(tailorMadeAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            });
            ColumnStateDefinitions.Add(combinedAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            });
            ColumnStateDefinitions.Add(manualAssemblyCategoryGroupIndex, new DataGridViewColumnStateDefinition());
        }

        private void UpdateDerivedData()
        {
            TryGetSimpleAssemblyCategoryGroup();
            TryGetDetailedAssemblyCategoryGroup();
            TryGetTailorMadeAssemblyCategoryGroup();
            TryGetCombinedAssemblyCategoryGroup();
        }

        private void TryGetSimpleAssemblyCategoryGroup()
        {
            try
            {
                simpleAssemblyCategoryGroup = StabilityStoneCoverFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(SectionResult);
            }
            catch (AssemblyException e)
            {
                simpleAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
            }
        }

        private void TryGetDetailedAssemblyCategoryGroup()
        {
            try
            {
                detailedAssemblyCategoryGroup = StabilityStoneCoverFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssessment(SectionResult);
            }
            catch (AssemblyException e)
            {
                detailedAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
            }
        }

        private void TryGetTailorMadeAssemblyCategoryGroup()
        {
            try
            {
                tailorMadeAssemblyCategoryGroup = StabilityStoneCoverFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(SectionResult);
            }
            catch (AssemblyException e)
            {
                tailorMadeAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
            }
        }

        private void TryGetCombinedAssemblyCategoryGroup()
        {
            try
            {
                combinedAssemblyCategoryGroup = StabilityStoneCoverFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(
                    SectionResult);
            }
            catch (AssemblyException e)
            {
                combinedAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
            }
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="StabilityStoneCoverSectionResultRow"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Sets the simple assessment result index.
            /// </summary>
            public int SimpleAssessmentResultIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment result for factorized signaling norm index.
            /// </summary>
            public int DetailedAssessmentResultForFactorizedSignalingNormIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment result for signaling norm index.
            /// </summary>
            public int DetailedAssessmentResultForSignalingNormIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment result for mechanism specific lower limit norm index.
            /// </summary>
            public int DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment result for lower limit norm index.
            /// </summary>
            public int DetailedAssessmentResultForLowerLimitNormIndex { internal get; set; }

            /// <summary>
            /// Sets the detailed assessment result for factorized lower limit norm index.
            /// </summary>
            public int DetailedAssessmentResultForFactorizedLowerLimitNormIndex { internal get; set; }
            
            /// <summary>
            /// Sets the tailor made assessment result index.
            /// </summary>
            public int TailorMadeAssessmentResultIndex { internal get; set; }
            
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
            /// Sets the manual assembly category group index.
            /// </summary>
            public int ManualAssemblyCategoryGroupIndex { internal get; set; }
        }
    }
}