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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.ClosingStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="CalculationGroup"/>
    /// in order to be able to create configurable closing structures calculations.
    /// </summary>
    public class ClosingStructuresCalculationGroupContext : ClosingStructuresContextBase<CalculationGroup>,
                                                            ICalculationContext<CalculationGroup, ClosingStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationGroupContext"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/> instance that is wrapped by this context object.</param>
        /// <param name="failureMechanism">The failure mechanism which contains the calculation group.</param>
        /// <param name="assessmentSection">The assessment section which containts the calculation group.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters are <c>null</c>.</exception>
        public ClosingStructuresCalculationGroupContext(CalculationGroup calculationGroup,
                                                        ClosingStructuresFailureMechanism failureMechanism,
                                                        IAssessmentSection assessmentSection)
            : base(calculationGroup, failureMechanism, assessmentSection) {}
    }
}