// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Forms.PresentationObjects
{
    /// <summary>
    /// The presentation object for an <see cref="StructureCollection{TStructure}"/> containing <see cref="StabilityPointStructure"/>.
    /// </summary>
    public class StabilityPointStructuresContext : ObservableWrappedObjectContextBase<StructureCollection<StabilityPointStructure>>
    {
        /// <summary>
        /// Creates an instance of <see cref="StabilityPointStructuresContext"/>.
        /// </summary>
        /// <param name="stabilityPointStructures">The wrapped <see cref="StructureCollection{T}"/> 
        /// containing <see cref="StabilityPointStructure"/>.</param>
        /// <param name="failureMechanism">The stability point structures failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section which the <paramref name="stabilityPointStructures"/> 
        /// belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input arguments are <c>null</c>.</exception>
        public StabilityPointStructuresContext(StructureCollection<StabilityPointStructure> stabilityPointStructures,
                                               StabilityPointStructuresFailureMechanism failureMechanism,
                                               IAssessmentSection assessmentSection)
            : base(stabilityPointStructures)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            AssessmentSection = assessmentSection;
            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the assessment section of this instance.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the parent failure mechanism of this instance.
        /// </summary>
        public StabilityPointStructuresFailureMechanism FailureMechanism { get; }
    }
}