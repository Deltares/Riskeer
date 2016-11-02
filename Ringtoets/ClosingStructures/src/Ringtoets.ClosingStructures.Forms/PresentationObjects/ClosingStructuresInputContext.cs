﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.ClosingStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="ClosingStructuresInput"/>
    /// in order to be able to configure closing structures calculations.
    /// </summary>
    public class ClosingStructuresInputContext : InputContextBase<ClosingStructuresInput, StructuresCalculation<ClosingStructuresInput>, ClosingStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculation input wrapped by the context object.</param>
        /// <param name="calculation">The closing structures calculation containing the <see cref="ClosingStructuresInput"/>.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public ClosingStructuresInputContext(ClosingStructuresInput wrappedData,
                                             StructuresCalculation<ClosingStructuresInput> calculation,
                                             ClosingStructuresFailureMechanism failureMechanism,
                                             IAssessmentSection assessmentSection)
            : base(wrappedData, calculation, failureMechanism, assessmentSection) {}
    }
}