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
using System.Collections.Generic;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.Revetment.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput();

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestWaveConditionsInputContext(waveConditionsInput,
                                                                         null,
                                                                         assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(waveConditionsInput);

            // Call
            TestDelegate call = () => new TestWaveConditionsInputContext(waveConditionsInput,
                                                                         calculation,
                                                                         null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ValidInput_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(waveConditionsInput);
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput,
                                                             calculation,
                                                             assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<TestWaveConditionsInput>>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, context.HydraulicBoundaryLocations);
        }

        private class TestWaveConditionsInputContext : WaveConditionsInputContext<TestWaveConditionsInput>
        {
            public TestWaveConditionsInputContext(TestWaveConditionsInput wrappedData,
                                                  ICalculation<TestWaveConditionsInput> calculation,
                                                  IAssessmentSection assessmentSection)
                : base(wrappedData, calculation, assessmentSection) {}

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
        }
    }
}