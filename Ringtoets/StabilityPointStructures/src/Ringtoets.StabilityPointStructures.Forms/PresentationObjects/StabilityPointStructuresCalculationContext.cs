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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="StabilityPointStructuresCalculation"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class StabilityPointStructuresCalculationContext : StabilityPointStructuresContextBase<StabilityPointStructuresCalculation>,
                                                              ICalculationContext<
                                                                  StabilityPointStructuresCalculation,
                                                                  StabilityPointStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationContext"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="StabilityPointStructuresCalculation"/> instance wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">When any input argument is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationContext(StabilityPointStructuresCalculation calculation,
                                                          StabilityPointStructuresFailureMechanism failureMechanism,
                                                          IAssessmentSection assessmentSection)
            : base(calculation, failureMechanism, assessmentSection) {}
    }
}