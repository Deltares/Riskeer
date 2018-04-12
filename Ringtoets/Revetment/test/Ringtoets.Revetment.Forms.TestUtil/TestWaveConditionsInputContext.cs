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

using System.Collections.Generic;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.Revetment.Forms.TestUtil
{
    /// <summary>
    /// Simple <see cref="WaveConditionsInputContext"/> implementation which can be used
    /// for test purposes.
    /// </summary>
    public class TestWaveConditionsInputContext : WaveConditionsInputContext
    {
        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>
        public TestWaveConditionsInputContext(WaveConditionsInput wrappedData)
            : this(wrappedData,
                   new ForeshoreProfile[0],
                   new AssessmentSectionStub()) {}

        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles.</param>
        /// <param name="assesmentSection">The assessment section.</param>
        public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                              IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                              IAssessmentSection assesmentSection)
            : this(wrappedData,
                   new TestWaveConditionsCalculation(),
                   assesmentSection,
                   foreshoreProfiles,
                   assesmentSection.HydraulicBoundaryDatabase.Locations) {}

        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>
        /// <param name="calculation">The calculation.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles.</param>
        /// <param name="locations">The hydraulic boundary locations.</param>
        public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                              ICalculation<WaveConditionsInput> calculation,
                                              IAssessmentSection assessmentSection,
                                              IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                              IEnumerable<HydraulicBoundaryLocation> locations)
            : base(wrappedData, calculation, assessmentSection)
        {
            ForeshoreProfiles = foreshoreProfiles;
        }

        public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}