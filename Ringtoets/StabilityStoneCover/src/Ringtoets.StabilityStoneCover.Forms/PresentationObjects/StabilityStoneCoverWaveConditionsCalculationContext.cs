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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationContext : StabilityStoneCoverContext<StabilityStoneCoverWaveConditionsCalculation>,
                                                                       ICalculationContext<StabilityStoneCoverWaveConditionsCalculation, StabilityStoneCoverFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityStoneCoverWaveConditionsCalculationContext"/> class.
        /// </summary>
        /// <param name="wrappedData">The <see cref="StabilityStoneCoverWaveConditionsCalculation"/> wrapped by this context object.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationContext(StabilityStoneCoverWaveConditionsCalculation wrappedData,
                                                                   CalculationGroup parent,
                                                                   StabilityStoneCoverFailureMechanism failureMechanism,
                                                                   IAssessmentSection assessmentSection)
            : base(wrappedData, failureMechanism, assessmentSection)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
        }

        public CalculationGroup Parent { get; }

        public override bool Equals(WrappedObjectContextBase<StabilityStoneCoverWaveConditionsCalculation> other)
        {
            return base.Equals(other)
                   && other is StabilityStoneCoverWaveConditionsCalculationContext
                   && ReferenceEquals(Parent, ((StabilityStoneCoverWaveConditionsCalculationContext)other).Parent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StabilityStoneCoverWaveConditionsCalculationContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Parent.GetHashCode();
        }
    }
}