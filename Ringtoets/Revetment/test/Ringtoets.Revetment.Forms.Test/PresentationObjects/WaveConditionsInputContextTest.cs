// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
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
            var waveConditionsInput = new WaveConditionsInput();
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Call
            TestDelegate call = () => new TestWaveConditionsInputContext(waveConditionsInput,
                                                                         null,
                                                                         assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();

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
            var waveConditionsInput = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput,
                                                             calculation,
                                                             assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<WaveConditionsInput>>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
        }

        private class TestWaveConditionsInputContext : WaveConditionsInputContext
        {
            public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                                  ICalculation<WaveConditionsInput> calculation,
                                                  IAssessmentSection assessmentSection)
                : base(wrappedData, calculation, assessmentSection) {}

            public override IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations { get; }

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
        }
    }
}