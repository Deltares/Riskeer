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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for the <see cref="WaveConditionsInput"/>.
    /// </summary>
    public class WaveConditionsInputContext : ObservableWrappedObjectContextBase<WaveConditionsInput>, IWaveConditionsInputContext
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveConditionsInputContext"/> class.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>
        /// <param name="foreshoreProfiles"></param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public WaveConditionsInputContext(WaveConditionsInput wrappedData,
                                          IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                          IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException("foreshoreProfiles");
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            ForeshoreProfiles = foreshoreProfiles;
            this.assessmentSection = assessmentSection;
        }

        public IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations
        {
            get
            {
                return assessmentSection.HydraulicBoundaryDatabase != null ?
                           assessmentSection.HydraulicBoundaryDatabase.Locations :
                           Enumerable.Empty<HydraulicBoundaryLocation>();
            }
        }

        public IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; private set; }
    }
}