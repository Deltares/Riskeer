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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.ClosingStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure a closing structures calculation.
    /// </summary>
    public class ClosingStructuresCalculationScenarioContext : StructuresCalculationContext<ClosingStructuresInput, ClosingStructuresFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationScenarioContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculation instance wrapped by this context object.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation.</param>
        /// <param name="failureMechanism">The failure mechanism which the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public ClosingStructuresCalculationScenarioContext(StructuresCalculation<ClosingStructuresInput> wrappedData,
                                                           CalculationGroup parent,
                                                           ClosingStructuresFailureMechanism failureMechanism,
                                                           IAssessmentSection assessmentSection)
            : base(wrappedData, parent, failureMechanism, assessmentSection) {}
    }
}