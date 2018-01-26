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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class DerivedMacroStabilityInwardsOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => DerivedMacroStabilityInwardsOutputFactory.Create(null,
                                                                                       new MacroStabilityInwardsFailureMechanism(), 
                                                                                       assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => DerivedMacroStabilityInwardsOutputFactory.Create(MacroStabilityInwardsOutputTestFactory.CreateOutput(),
                                                                        null,
                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DerivedMacroStabilityInwardsOutputFactory.Create(MacroStabilityInwardsOutputTestFactory.CreateOutput(),
                                                                        new MacroStabilityInwardsFailureMechanism(),
                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_ValidData_ReturnsExpectedValue()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                MacroStabilityInwardsProbabilityAssessmentInput =
                {
                    SectionLength = 6000
                },
                Contribution = 100
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);

            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            // Call
            DerivedMacroStabilityInwardsOutput derivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(output, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(0.697, derivedOutput.FactorOfStability, derivedOutput.FactorOfStability.GetAccuracy());
            Assert.AreEqual(0.707, derivedOutput.MacroStabilityInwardsFactorOfSafety, derivedOutput.MacroStabilityInwardsFactorOfSafety.GetAccuracy());
            Assert.AreEqual(0.073605149538226278, derivedOutput.MacroStabilityInwardsProbability, 1e-6);
            Assert.AreEqual(1.44946, derivedOutput.MacroStabilityInwardsReliability, derivedOutput.MacroStabilityInwardsReliability.GetAccuracy());
            Assert.AreEqual(0.020161290322580648d, derivedOutput.RequiredProbability, 1e-6);
            Assert.AreEqual(2.05043, derivedOutput.RequiredReliability, derivedOutput.RequiredReliability.GetAccuracy());
            mocks.VerifyAll();
        }
    }
}