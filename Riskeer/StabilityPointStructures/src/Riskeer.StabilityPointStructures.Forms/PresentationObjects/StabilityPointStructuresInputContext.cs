// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="StabilityPointStructuresInput"/>
    /// in order to be able to configure stability point structures calculations.
    /// </summary>
    public class StabilityPointStructuresInputContext : InputContextBase<StabilityPointStructuresInput,
        StructuresCalculation<StabilityPointStructuresInput>,
        StabilityPointStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresInputContext"/>.
        /// </summary>
        /// <param name="input">The <see cref="StabilityPointStructuresInput"/> instance wrapped by this context object.</param>
        /// <param name="calculation">The calculation item which the <paramref name="input"/> belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public StabilityPointStructuresInputContext(StabilityPointStructuresInput input,
                                                    StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                    StabilityPointStructuresFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection)
            : base(input, calculation, failureMechanism, assessmentSection) {}
    }
}