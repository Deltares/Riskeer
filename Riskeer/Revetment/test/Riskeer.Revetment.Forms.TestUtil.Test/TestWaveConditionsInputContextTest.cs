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

using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.Revetment.Forms.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_WithInput_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext<TestWaveConditionsInput>>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.IsInstanceOf<TestWaveConditionsCalculation<TestWaveConditionsInput>>(context.Calculation);
            Assert.IsInstanceOf<AssessmentSectionStub>(context.AssessmentSection);
            CollectionAssert.IsEmpty(context.ForeshoreProfiles);
        }

        [Test]
        public void Constructor_WithInputAndForeshoreProfilesAndAssessmentSection_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput();
            var profiles = new ForeshoreProfile[0];
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput, profiles, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext<TestWaveConditionsInput>>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.IsInstanceOf<TestWaveConditionsCalculation<TestWaveConditionsInput>>(context.Calculation);
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

            var waveConditionsInput = new TestWaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(waveConditionsInput);
            var profiles = new ForeshoreProfile[0];

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput,
                                                             calculation,
                                                             assessmentSection,
                                                             profiles);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext<TestWaveConditionsInput>>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(profiles, context.ForeshoreProfiles);
            mocks.VerifyAll();
        }
    }
}