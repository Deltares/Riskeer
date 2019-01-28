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
using System.Collections.Generic;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="CalculationGroup"/>
    /// in order be able to create configurable stability point structures calculations.
    /// </summary>
    public class StabilityPointStructuresCalculationGroupContext : FailureMechanismItemContextBase<CalculationGroup, StabilityPointStructuresFailureMechanism>,
                                                                   ICalculationContext<CalculationGroup, StabilityPointStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationGroupContext"/>.
        /// </summary>
        /// <param name="calculationsGroup">The <see cref="CalculationGroup"/> instance wrapped by this context object.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation group.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>, except for <paramref name="parent"/>.</exception>
        public StabilityPointStructuresCalculationGroupContext(CalculationGroup calculationsGroup,
                                                               CalculationGroup parent,
                                                               StabilityPointStructuresFailureMechanism failureMechanism,
                                                               IAssessmentSection assessmentSection)
            : base(calculationsGroup, failureMechanism, assessmentSection)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets the available foreshore profiles.
        /// </summary>
        public IEnumerable<ForeshoreProfile> AvailableForeshoreProfiles
        {
            get
            {
                return FailureMechanism.ForeshoreProfiles;
            }
        }

        /// <summary>
        /// Gets the available stability point structures.
        /// </summary>
        public IEnumerable<StabilityPointStructure> AvailableStructures
        {
            get
            {
                return FailureMechanism.StabilityPointStructures;
            }
        }

        public CalculationGroup Parent { get; }

        public override bool Equals(WrappedObjectContextBase<CalculationGroup> other)
        {
            return base.Equals(other)
                   && other is StabilityPointStructuresCalculationGroupContext
                   && ReferenceEquals(Parent, ((StabilityPointStructuresCalculationGroupContext) other).Parent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StabilityPointStructuresCalculationGroupContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Parent?.GetHashCode() ?? 0;
        }
    }
}