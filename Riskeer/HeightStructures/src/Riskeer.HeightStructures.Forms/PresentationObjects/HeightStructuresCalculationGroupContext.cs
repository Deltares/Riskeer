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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="CalculationGroup"/>
    /// in order be able to create configurable height structures calculations.
    /// </summary>
    public class HeightStructuresCalculationGroupContext : FailureMechanismItemContextBase<CalculationGroup, HeightStructuresFailureMechanism>,
                                                           ICalculationContext<CalculationGroup, HeightStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationGroupContext"/>.
        /// </summary>
        /// <param name="calculationsGroup">The <see cref="CalculationGroup"/> instance wrapped by this context object.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation group.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>, except for <paramref name="parent"/>.</exception>
        public HeightStructuresCalculationGroupContext(CalculationGroup calculationsGroup,
                                                       CalculationGroup parent,
                                                       HeightStructuresFailureMechanism failureMechanism,
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
        /// Gets the available height structures.
        /// </summary>
        public IEnumerable<HeightStructure> AvailableStructures
        {
            get
            {
                return FailureMechanism.HeightStructures;
            }
        }

        public CalculationGroup Parent { get; }

        public override bool Equals(WrappedObjectContextBase<CalculationGroup> other)
        {
            return base.Equals(other)
                   && other is HeightStructuresCalculationGroupContext
                   && ReferenceEquals(Parent, ((HeightStructuresCalculationGroupContext) other).Parent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HeightStructuresCalculationGroupContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Parent?.GetHashCode() ?? 0;
        }
    }
}