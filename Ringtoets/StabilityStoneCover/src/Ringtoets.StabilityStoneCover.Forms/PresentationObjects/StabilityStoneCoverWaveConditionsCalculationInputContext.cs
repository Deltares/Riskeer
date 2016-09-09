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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for the input of <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationInputContext : StabilityStoneCoverContext<WaveConditionsInput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityStoneCoverWaveConditionsCalculationInputContext"/> class.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>
        /// <param name="failureMechanism">The failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationInputContext(WaveConditionsInput wrappedData,
                                                                        StabilityStoneCoverFailureMechanism failureMechanism,
                                                                        IAssessmentSection assessmentSection) :
                                                                            base(wrappedData, failureMechanism, assessmentSection) {}

        /// <summary>
        /// Gets the hydraulic boundary locations.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
        {
            get
            {
                return AssessmentSection.HydraulicBoundaryDatabase != null ?
                           AssessmentSection.HydraulicBoundaryDatabase.Locations :
                           Enumerable.Empty<HydraulicBoundaryLocation>();
            }
        }

        /// <summary>
        /// Gets the foreshore profiles.
        /// </summary>
        public IEnumerable<ForeshoreProfile> ForeshoreProfiles
        {
            get
            {
                return FailureMechanism.ForeshoreProfiles;
            }
        }
    }
}