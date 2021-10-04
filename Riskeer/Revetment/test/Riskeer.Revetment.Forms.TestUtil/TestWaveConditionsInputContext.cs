﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.Revetment.Forms.TestUtil
{
    /// <summary>
    /// Simple <see cref="WaveConditionsInputContext{T}"/> implementation which can be used
    /// for test purposes.
    /// </summary>
    public class TestWaveConditionsInputContext : WaveConditionsInputContext<WaveConditionsInput>
    {
        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        public TestWaveConditionsInputContext(WaveConditionsInput wrappedData)
            : this(wrappedData,
                   Array.Empty<ForeshoreProfile>(),
                   new AssessmentSectionStub()) {}

        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                              IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                              IAssessmentSection assessmentSection)
            : this(wrappedData,
                   new TestWaveConditionsCalculation<WaveConditionsInput>(wrappedData),
                   assessmentSection,
                   foreshoreProfiles) {}

        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        /// <param name="calculation">The calculation.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles.</param>
        public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                              ICalculation<WaveConditionsInput> calculation,
                                              IAssessmentSection assessmentSection,
                                              IEnumerable<ForeshoreProfile> foreshoreProfiles)
            : base(wrappedData, calculation, assessmentSection)
        {
            ForeshoreProfiles = foreshoreProfiles;
        }

        public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}