// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.PresentationObjects;

namespace Riskeer.Revetment.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new WaveConditionsInputContext(waveConditionsInput, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<WaveConditionsInput>(waveConditionsInput);

            // Call
            void Call() => new WaveConditionsInputContext(waveConditionsInput, calculation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ValidInput_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation<WaveConditionsInput>(waveConditionsInput);
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var context = new WaveConditionsInputContext(waveConditionsInput,
                                                         calculation,
                                                         assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<WaveConditionsInput>>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, context.HydraulicBoundaryLocations);
        }

        private class WaveConditionsInputContext : WaveConditionsInputContext<WaveConditionsInput>
        {
            public WaveConditionsInputContext(WaveConditionsInput wrappedData,
                                              ICalculation<WaveConditionsInput> calculation,
                                              IAssessmentSection assessmentSection)
                : base(wrappedData, calculation, assessmentSection) {}

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
        }
    }
}