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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for configuration of scenarios for the height structures
    /// failure mechanism.
    /// </summary>
    public class HeightStructuresScenariosContext : WrappedObjectContextBase<CalculationGroup>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresScenariosContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="CalculationGroup"/>.</param>
        /// <param name="failureMechanism">A <see cref="HeightStructuresFailureMechanism"/> forming the context.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the <paramref name="wrappedData"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresScenariosContext(CalculationGroup wrappedData, HeightStructuresFailureMechanism failureMechanism,
                                                IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            ParentFailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// The parent failure mechanism of the calculation group.
        /// </summary>
        public HeightStructuresFailureMechanism ParentFailureMechanism { get; }

        /// <summary>
        /// Gets the <see cref="IAssessmentSection"/> of the calculation group.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}