// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationContext : GrassCoverErosionOutwardsContext<GrassCoverErosionOutwardsWaveConditionsCalculation>,
                                                                             ICalculationContext<GrassCoverErosionOutwardsWaveConditionsCalculation, GrassCoverErosionOutwardsFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationContext"/> class.
        /// </summary>
        /// <param name="calculation">The wrapped <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveConditionsCalculationContext(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                         CalculationGroup parent,
                                                                         GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                         IAssessmentSection assessmentSection)
            : base(calculation, failureMechanism, assessmentSection)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
        }

        public CalculationGroup Parent { get; }

        public override bool Equals(WrappedObjectContextBase<GrassCoverErosionOutwardsWaveConditionsCalculation> other)
        {
            return base.Equals(other)
                   && other is GrassCoverErosionOutwardsWaveConditionsCalculationContext
                   && ReferenceEquals(Parent, ((GrassCoverErosionOutwardsWaveConditionsCalculationContext) other).Parent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GrassCoverErosionOutwardsWaveConditionsCalculationContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Parent.GetHashCode();
        }
    }
}