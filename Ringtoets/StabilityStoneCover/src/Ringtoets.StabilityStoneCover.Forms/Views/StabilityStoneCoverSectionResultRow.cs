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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;
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
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverSectionResultRow"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>
        /// to wrap so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public StabilityStoneCoverSectionResultRow(StabilityStoneCoverFailureMechanismSectionResult sectionResult) : base(sectionResult) {}

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
        /// Gets the assessment layer two a of the <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>.
        /// </summary>
        public AssessmentLayerTwoAResult AssessmentLayerTwoA
        {
            get
            {
                return AssessmentLayerTwoAResult.Successful;
            }
        }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble AssessmentLayerThree
        {
            get
            {
                return SectionResult.AssessmentLayerThree;
            }
            set
            {
                SectionResult.AssessmentLayerThree = value.ToPrecision(SectionResult.AssessmentLayerThree.NumberOfDecimalPlaces);
            }
        }

        public override void Update() {}
    }
}