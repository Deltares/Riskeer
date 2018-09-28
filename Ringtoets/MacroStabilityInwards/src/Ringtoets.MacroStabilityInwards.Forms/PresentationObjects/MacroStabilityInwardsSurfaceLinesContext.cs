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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Forms.PresentationObjects
{
    /// <summary>
    /// The presentation object for <see cref="MacroStabilityInwardsFailureMechanism.SurfaceLines"/>.
    /// </summary>
    public class MacroStabilityInwardsSurfaceLinesContext : ObservableWrappedObjectContextBase<MacroStabilityInwardsSurfaceLineCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSurfaceLinesContext"/>.
        /// </summary>
        /// <param name="surfaceLines">The collection to update.</param>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public MacroStabilityInwardsSurfaceLinesContext(MacroStabilityInwardsSurfaceLineCollection surfaceLines,
                                                        MacroStabilityInwardsFailureMechanism failureMechanism,
                                                        IAssessmentSection assessmentSection)
            : base(surfaceLines)
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
        /// Gets the assessment section which the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the failure mechanism which the context belongs to.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism FailureMechanism { get; }
    }
}