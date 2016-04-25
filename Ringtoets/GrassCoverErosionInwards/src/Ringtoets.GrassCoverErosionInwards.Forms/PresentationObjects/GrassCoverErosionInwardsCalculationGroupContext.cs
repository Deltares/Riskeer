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
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="CalculationGroup"/>
    /// in order be able to create configurable grass cover erosion inwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationGroupContext : GrassCoverErosionInwardsContext<ICalculationGroup>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationGroupContext"/>.
        /// </summary>
        /// <param name="calculationsGroup">The <see cref="ICalculationGroup"/> instance wrapped by this context object.</param>
        /// <param name="grassCoverErosionInwardsFailureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        public GrassCoverErosionInwardsCalculationGroupContext(ICalculationGroup calculationsGroup, GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism, IAssessmentSection assessmentSection)
            : base(calculationsGroup, assessmentSection)
        {
            if (grassCoverErosionInwardsFailureMechanism == null)
            {
                var message = string.Format(Resources.GrassCoverErosionInwardsContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                            Resources.GrassCoverErosionInwardsContext_DataDescription_GrassCoverErosionInwardsFailureMechanism);
                throw new ArgumentNullException("grassCoverErosionInwardsFailureMechanism", message);
            }

            GrassCoverErosionInwardsFailureMechanism = grassCoverErosionInwardsFailureMechanism;
        }

        /// <summary>
        /// Gets the <see cref="GrassCoverErosionInwardsFailureMechanism"/> which the <see cref="GrassCoverErosionInwardsCalculationGroupContext"/> belongs to.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwardsFailureMechanism { get; private set; }
    }
}