﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Data;

namespace Riskeer.StabilityStoneCover.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="StabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationGroupContext : StabilityStoneCoverContext<CalculationGroup>,
                                                                            ICalculationContext<CalculationGroup, StabilityStoneCoverFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityStoneCoverWaveConditionsCalculationGroupContext"/> class.
        /// </summary>
        /// <param name="calculationGroup">The wrapped <see cref="CalculationGroup"/>.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation group.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation group belongs to.</param>
        /// <param name="assessmentSection">The assessment section  the calculation group belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>, except for <paramref name="parent"/>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationGroupContext(CalculationGroup calculationGroup,
                                                                        CalculationGroup parent,
                                                                        StabilityStoneCoverFailureMechanism failureMechanism,
                                                                        IAssessmentSection assessmentSection)
            : base(calculationGroup, failureMechanism, assessmentSection)
        {
            Parent = parent;
        }

        public CalculationGroup Parent { get; }

        public override bool Equals(WrappedObjectContextBase<CalculationGroup> other)
        {
            return base.Equals(other)
                   && other is StabilityStoneCoverWaveConditionsCalculationGroupContext
                   && ReferenceEquals(Parent, ((StabilityStoneCoverWaveConditionsCalculationGroupContext) other).Parent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StabilityStoneCoverWaveConditionsCalculationGroupContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Parent?.GetHashCode() ?? 0;
        }
    }
}