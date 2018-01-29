﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="WaveConditionsInput"/> for the <see cref="StabilityStoneCoverFailureMechanismContext"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsInputContext : WaveConditionsInputContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>>
        /// <param name="calculation">The calculation having <paramref name="wrappedData"/> as input.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles of the <see cref="StabilityStoneCoverFailureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                                             ICalculation<WaveConditionsInput> calculation,
                                                             IAssessmentSection assessmentSection,
                                                             IEnumerable<ForeshoreProfile> foreshoreProfiles)
            : base(wrappedData, calculation, assessmentSection)
        {
            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }

            ForeshoreProfiles = foreshoreProfiles;
        }

        public override IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
        {
            get
            {
                return AssessmentSection.HydraulicBoundaryDatabase.Locations;
            }
        }

        public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}