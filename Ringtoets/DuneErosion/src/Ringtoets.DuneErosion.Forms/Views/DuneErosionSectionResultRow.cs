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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// Class for displaying <see cref="DuneErosionFailureMechanismSectionResult"/> as a row in a grid view.
    /// </summary>
    public class DuneErosionSectionResultRow : FailureMechanismSectionResultRow<DuneErosionFailureMechanismSectionResult>
    {
        private FailureMechanismSectionAssemblyCategoryGroup simpleAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup detailedAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssemblyCategoryGroup;
        private FailureMechanismSectionAssemblyCategoryGroup combinedAssemblyCategoryGroup;

        /// <summary>
        /// Creates a new instance of <see cref="DuneErosionSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="DuneErosionFailureMechanismSectionResult"/> to wrap
        /// so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public DuneErosionSectionResultRow(DuneErosionFailureMechanismSectionResult sectionResult)
            : base(sectionResult)
        {
            Update();
        }

        /// <summary>
        /// Gets or sets the value representing the simple assessment result.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public SimpleAssessmentValidityOnlyResultType SimpleAssessmentResult
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
        /// Gets or sets the value of the detailed assessment of safety per failure mechanism section
        /// for the factorized signaling norm (Cat Iv - IIv).
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
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
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
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
        /// for the failure mechanism specific lower limit norm (Cat IIIv - IVv).
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
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
        /// for the lower limit norm (Cat IVv - Vv).
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
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
        /// for the factorized lower limit norm (Cat Vv - VIv).
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
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
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
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

        public override void Update()
        {
            UpdateDerivedData();
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
                simpleAssemblyCategoryGroup = DuneErosionFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(SectionResult);
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
                detailedAssemblyCategoryGroup = DuneErosionFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssessment(SectionResult);
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
                tailorMadeAssemblyCategoryGroup = DuneErosionFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssessment(SectionResult);
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
                combinedAssemblyCategoryGroup = DuneErosionFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssessment(
                    SectionResult);
            }
            catch (AssemblyException e)
            {
                combinedAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
            }
        }
    }
}