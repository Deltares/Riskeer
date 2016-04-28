// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects
{
    public class GrassCoverErosionInwardsInputContext : GrassCoverErosionInwardsContext<GrassCoverErosionInwardsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputContext"/>
        /// </summary>
        /// <param name="input">The grass cover erosion inwards input instance wrapped by this context object.</param>
        /// <param name="calculation">The calculation item the <paramref name="input"/> belongs to.</param>
        /// <param name="grassCoverErosionInwardsFailureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">When any input parameter is null.</exception>
        public GrassCoverErosionInwardsInputContext(GrassCoverErosionInwardsInput input,
                                                    ICalculation calculation,
                                                    GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism,
                                                    IAssessmentSection assessmentSection)
            : base(input, assessmentSection)
        {
            if (calculation == null)
            {
                var message = String.Format(Resources.GrassCoverErosionInwardsContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.GrassCoverErosionInwardsInputContext_DataDescription_GrassCoverErosionInwardsInputCalculationItem);

                throw new ArgumentNullException("calculation", message);
            }
            if (grassCoverErosionInwardsFailureMechanism == null)
            {
                var message = String.Format(Resources.GrassCoverErosionInwardsContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.GrassCoverErosionInwardsContext_DataDescription_GrassCoverErosionInwardsFailureMechanism);

                throw new ArgumentNullException("grassCoverErosionInwardsFailureMechanism", message);
            }

            Calculation = calculation;

            GrassCoverErosionInwardsFailureMechanism = grassCoverErosionInwardsFailureMechanism;
        }

        /// <summary>
        /// Gets the calculation item which the context belongs to.
        /// </summary>
        public ICalculation Calculation { get; private set; }

        /// <summary>
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwardsFailureMechanism { get; private set; }
    }
}