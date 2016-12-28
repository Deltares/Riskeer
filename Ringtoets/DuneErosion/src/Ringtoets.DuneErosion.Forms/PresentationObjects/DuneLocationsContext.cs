// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for Hydraulic boundary locations.
    /// </summary>
    public class DuneLocationsContext : ObservableWrappedObjectContextBase<ObservableList<DuneLocation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsContext"/>.
        /// </summary>
        /// <param name="duneLocations">The dune locations for dune erosion failure mechanism.</param>
        /// <param name="failureMechanism">The dune erosion failure mechanism which the locations belong to.</param>
        /// <param name="assessmentSection">The assessment section the locations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public DuneLocationsContext(ObservableList<DuneLocation> duneLocations,
                                                 DuneErosionFailureMechanism failureMechanism,
                                                 IAssessmentSection assessmentSection)
            : base(duneLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }
            AssessmentSection = assessmentSection;
            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; private set; }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism { get; private set; }
    }
}