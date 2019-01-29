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

using System.Collections.Generic;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Riskeer.Revetment.Forms.TestUtil
{
    /// <summary>
    /// Simple <see cref="WaveConditionsInputContext{T}"/> implementation which can be used
    /// for test purposes.
    /// </summary>
    public class TestWaveConditionsInputContext : WaveConditionsInputContext<TestWaveConditionsInput>
    {
        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        public TestWaveConditionsInputContext(TestWaveConditionsInput wrappedData)
            : this(wrappedData,
                   new ForeshoreProfile[0],
                   new AssessmentSectionStub()) {}

        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        public TestWaveConditionsInputContext(TestWaveConditionsInput wrappedData,
                                              IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                              IAssessmentSection assessmentSection)
            : this(wrappedData,
                   new TestWaveConditionsCalculation<TestWaveConditionsInput>(wrappedData),
                   assessmentSection,
                   foreshoreProfiles) {}

        /// <summary>
        /// Creates a new <see cref="TestWaveConditionsInputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped wave conditions input.</param>
        /// <param name="calculation">The calculation.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles.</param>
        public TestWaveConditionsInputContext(TestWaveConditionsInput wrappedData,
                                              ICalculation<TestWaveConditionsInput> calculation,
                                              IAssessmentSection assessmentSection,
                                              IEnumerable<ForeshoreProfile> foreshoreProfiles)
            : base(wrappedData, calculation, assessmentSection)
        {
            ForeshoreProfiles = foreshoreProfiles;
        }

        public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}