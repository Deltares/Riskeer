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

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.Revetment.Forms.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_WithInput_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.IsInstanceOf<TestWaveConditionsCalculation>(context.Calculation);
            Assert.IsInstanceOf<AssessmentSectionStub>(context.AssessmentSection);
            CollectionAssert.IsEmpty(context.ForeshoreProfiles);
            CollectionAssert.IsEmpty(context.HydraulicBoundaryLocations);
        }

        [Test]
        public void Constructor_WithInputAndForeshoreProfilesAndAssessmentSection_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();
            var profiles = new ForeshoreProfile[0];
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput, profiles, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.IsInstanceOf<TestWaveConditionsCalculation>(context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(profiles, context.ForeshoreProfiles);
        }

        [Test]
        public void Constructor_WithAllParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var waveConditionsInput = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();
            var profiles = new ForeshoreProfile[0];
            var locations = new HydraulicBoundaryLocation[0];

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput,
                                                             calculation,
                                                             assessmentSection,
                                                             profiles);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(profiles, context.ForeshoreProfiles);
            mocks.VerifyAll();
        }
    }
}