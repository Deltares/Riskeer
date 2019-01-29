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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for the output of height structures calculations.
    /// </summary>
    public class HeightStructuresOutputContext : StructuresOutputContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresOutputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The structures calculation wrapped by the context object.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresOutputContext(IStructuresCalculation wrappedData,
                                             HeightStructuresFailureMechanism failureMechanism,
                                             IAssessmentSection assessmentSection)
            : base(wrappedData, assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public HeightStructuresFailureMechanism FailureMechanism { get; }
    }
}